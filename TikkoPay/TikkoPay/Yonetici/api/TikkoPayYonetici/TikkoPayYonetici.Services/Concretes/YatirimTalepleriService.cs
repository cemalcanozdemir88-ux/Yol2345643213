using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.CallBacks;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TikkoPayYonetici.Core;

namespace TikkoPayYonetici.Services.Concretes
{
    public class YatirimTalepleriService : IYatirimTalepleriService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IDepositTransactionRepository _depositTransactionRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly TransactionsApiSettings _transactionsApiSettings;

        public YatirimTalepleriService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IYatirimTalepleriRepository yatirimTalepleriRepository,
            ITedarikciRepository tedarikciRepository,
            IWebSiteleriRepository webSiteleriRepository,
            IDepositTransactionRepository depositTransactionRepository,
            IHavaleBankalariRepository havaleBankalariRepository,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository,
            ISettingsV2Repository settingsV2Repository,
            IOptions<TransactionsApiSettings> transactionsApiSettingsOptions)
        {
            _settingsV2Repository = settingsV2Repository;
            _transactionsApiSettings = transactionsApiSettingsOptions.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> GetById(Guid id)
        {
            var item = await _yatirimTalepleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<YatirimTalepleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> Create(YatirimTalepleriCreateDto item)
        {
            YatirimCekimTalepleri model = _mapper.Map<YatirimCekimTalepleri>(item);

            model.TalepNo = DateTime.Now.ToString("yyyyMMdd");
            model.PaymentMethodId = item.PaymentMethodId;

            model.YontemTipi = "Manuel";
            model.IslemTipi = IslemTip.Deposit;


            var userIdConvertResult = long.TryParse(model.UserId?.Trim(), out long userIdBigIntValue);
            if (userIdConvertResult)
            {
                model.UserIdBigIntValue = userIdBigIntValue;
            }

            var havaleBankalariItem = await _havaleBankalariRepository.GetByIdDtoAsync(item.HavaleBankaId!.Value);
            if (havaleBankalariItem is null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Havale bankası bulunamadı"
                };
            }

            model.BankaId = havaleBankalariItem.BankaId;
            model.HesapNo = havaleBankalariItem.HesapNo;
            model.HesapSahibi = havaleBankalariItem.HesapSahibi;

            if (model.TedarikciId.HasValue)
            {
                model.TedarikciKomisyon = await GetTedarikciYatirimKomisyon(model.TedarikciId.Value, model.Tutar);
            }

            if (model.WebSiteId.HasValue)
            {
                model.WebSiteKomisyon = await GetWebSiteYatirimKomisyon(model.WebSiteId.Value, model.Tutar);
            }

            model.Toplam = model.Tutar - model.TedarikciKomisyon;
            //model.Durum = Durum.Onaylandi;
            model.SilindiMi = false;

            if(model.Durum == Durum.Onaylandi || model.Durum == Durum.Red)
            {
                model.OnayTarihi = DateTime.Now;
                model.GuncellemeTarihi = DateTime.Now;
            }

            model.KayitTarihi = DateTime.Now;
            

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            model.GuncelleyenKullaniciId = kullaniciId;

            await _yatirimTalepleriRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true,
                    Result = new YatirimTalepleriListDto { Id = model.Id },
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> Update(YatirimTalepleriUpdateDto itemDto)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _yatirimTalepleriRepository.Update(dbItem, itemDto);
            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> BekleyeneAl(Guid id)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Red || dbItem.IslemTipi != IslemTip.Deposit)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "İptal edilen yatırımlar bekleyene alınabilir."
                };
            }

            dbItem.Durum = Durum.Bekliyor;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Yatırım talebi bekleyene alındı",
                Success = true
            };
        }

        /// <summary>
        /// İptal edilen yatırımı yeni tutar / tedarikçi ile yeni bir kayıt olarak tedarikçiye gönderir.
        /// Mevcut (iptal) kayıt olduğu gibi kalır; yeni kayıt TopId ile eski kayda bağlanır.
        /// WebSiteTransactionId aynı kalır (web sitesinden gelen transaction).
        /// </summary>
        public async Task<ResponseViewModel<YatirimTalepleriListDto>> YenidenYonlendir(YenidenYonlendirRequest request)
        {
            if (request.Tutar <= 0)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Tutar sıfırdan büyük olmalıdır."
                };
            }

            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(request.Id);
            if (dbItem == null || dbItem.SilindiMi)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Red || dbItem.IslemTipi != IslemTip.Deposit)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Sadece iptal edilen yatırımlar yeniden gönderilebilir."
                };
            }

            var tedarikciItem = await _tedarikciRepository.GetByIdAsync(request.TedarikciId);
            if (tedarikciItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Tedarikçi bulunamadı."
                };
            }

            // Aynı transaction için bekleyen ya da onaylanmış bir kayıt varsa tekrar açılmasın (çift onay riskini engeller)
            var aktifKayitVar = _yatirimTalepleriRepository.Find(k => k.WebSiteTransactionId == dbItem.WebSiteTransactionId &&
                                                                      k.IslemTipi == IslemTip.Deposit &&
                                                                      (k.Durum == Durum.Bekliyor || k.Durum == Durum.Onaylandi) &&
                                                                      !k.SilindiMi).Any();
            if (aktifKayitVar)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Bu transaction için zaten bekleyen veya onaylanmış bir kayıt var."
                };
            }

            // API'li tedarikçi (FixHavale, ViPay vb.) seçildiyse önce onların sistemine istek atılır.
            // Tedarikçi kabul etmezse hiçbir kayıt açılmaz.
            var settings = await _settingsV2Repository.GetSettingsAsync();
            var apiliTedarikci = IsApiliTedarikci(settings, request.TedarikciId);
            TedarikciHesapResponse? tedarikciHesabi = null;

            if (apiliTedarikci)
            {
                if (dbItem.TedarikciId == request.TedarikciId)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = "Bu işlem zaten bu tedarikçide reddedildi. API ile çalışan tedarikçiye aynı işlem tekrar gönderilemez."
                    };
                }

                tedarikciHesabi = await SendDepositToApiliTedarikciAsync(dbItem.WebSiteTransactionId, request.TedarikciId, request.Tutar);
                if (!tedarikciHesabi.Success)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = tedarikciHesabi.Message
                    };
                }
            }

            var nameidentifier = _httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            var yeniKayit = dbItem.DeepClone();

            yeniKayit.Id = Guid.NewGuid();
            yeniKayit.TopId = dbItem.Id; // eski (iptal) kayda bağlantı - history
            yeniKayit.TedarikciId = request.TedarikciId;
            yeniKayit.AdSoyadYatirimYapan = dbItem.AdSoyadYatirimYapan;
            yeniKayit.Tutar = request.Tutar;
            yeniKayit.TedarikciKomisyon = await GetTedarikciYatirimKomisyon(request.TedarikciId, request.Tutar);
            yeniKayit.WebSiteKomisyon = yeniKayit.WebSiteId.HasValue
                ? await GetWebSiteYatirimKomisyon(yeniKayit.WebSiteId.Value, request.Tutar)
                : 0;
            yeniKayit.Toplam = yeniKayit.Tutar - yeniKayit.TedarikciKomisyon;

            yeniKayit.Durum = Durum.Bekliyor;
            yeniKayit.IptalAciklama = string.Empty;
            yeniKayit.OnayTarihi = null;
            yeniKayit.IsSendToWebSite = false;
            yeniKayit.WebSiteCallBackResult = string.Empty;
            yeniKayit.SilindiMi = false;

            yeniKayit.KayitTarihi = DateTime.Now;
            yeniKayit.KaydedenKullaniciId = kullaniciId;
            yeniKayit.GuncellemeTarihi = null;
            yeniKayit.GuncelleyenKullaniciId = null;
            yeniKayit.YonlendirenKullaniciId = kullaniciId;
            yeniKayit.YonlendirilmeTarihi = DateTime.Now;

            // API'li tedarikçide hesap bilgisi tedarikçinin verdiği hesaptır (normal yatırım akışındaki gibi)
            if (tedarikciHesabi != null)
            {
                yeniKayit.HavaleBankaId = tedarikciHesabi.HavaleBankaId;
                if (tedarikciHesabi.BankaId.HasValue)
                {
                    yeniKayit.BankaId = tedarikciHesabi.BankaId;
                }
                yeniKayit.HesapNo = tedarikciHesabi.HesapNo;
                yeniKayit.HesapSahibi = tedarikciHesabi.HesapSahibi;
                yeniKayit.YontemTipi = tedarikciHesabi.YontemTipi;
            }

            // DB'de unique index var: UQ_WebSiteTransactionIdBigIntValue_Filtered ([IslemTipi]=1 AND [YontemTipi]<>'Manuel'), NULL dahil.
            // Callback'ler (TikkoPayCallbacks) bu kolonla eşleşen TÜM kayıtların tutar/komisyonunu günceller;
            // bu yüzden değer sadece aktif (yeni) kayıtta kalmalı.
            // Eski kayda -IdNumber yazılır: her kayıt için benzersizdir ve gerçek (pozitif) transaction id ile çakışmaz.
            // Eski kayıtta WebSiteTransactionId (string) aynen kalır, history bozulmaz.
            //
            // Not: dbItem daha önce de yeniden gönderilmişse üzerinde zaten negatif değer vardır ve DeepClone bunu
            // yeni kayda da kopyalar (duplicate key hatası). Bu yüzden yeni kayda her zaman gerçek değer verilir.
            long? gercekTransactionDegeri = dbItem.WebSiteTransactionIdBigIntValue > 0
                ? dbItem.WebSiteTransactionIdBigIntValue
                : (long.TryParse(dbItem.WebSiteTransactionId, out var parsedTransactionId) ? parsedTransactionId : null);

            if (gercekTransactionDegeri.HasValue)
            {
                // Gerçek değeri hâlâ taşıyan eski (iptal) kayıtlar varsa onları da kendi -IdNumber değerlerine çek
                var degeriTasiyanEskiKayitlar = _yatirimTalepleriRepository
                    .Find(k => k.WebSiteTransactionIdBigIntValue == gercekTransactionDegeri && k.Id != dbItem.Id)
                    .ToList();
                foreach (var eskiKayit in degeriTasiyanEskiKayitlar)
                {
                    eskiKayit.WebSiteTransactionIdBigIntValue = -eskiKayit.IdNumber;
                }
            }

            yeniKayit.WebSiteTransactionIdBigIntValue = gercekTransactionDegeri;
            dbItem.WebSiteTransactionIdBigIntValue = -dbItem.IdNumber;

            await _yatirimTalepleriRepository.AddAsync(yeniKayit);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true,
                    Message = $"Yeni kayıt oluşturuldu ve {tedarikciItem.Unvan} tedarikçisinin onayına gönderildi.",
                    Result = new YatirimTalepleriListDto { Id = yeniKayit.Id }
                };
            }

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = false,
                Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePriceAndYatirimYapanAdSoyad(Guid id, decimal price, string yatirimYapanAdSoyad)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Bekliyor)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = $"Bekliyor durumundaki kayıtların fiyatı düzenlenebilir. Bu kayıt {dbItem.Durum.ToString()} durumunda"
                };
            }

            if(dbItem.TedarikciId.HasValue)
            {
                dbItem.TedarikciKomisyon = await GetTedarikciYatirimKomisyon(dbItem.TedarikciId.Value, price);
            }

            if (dbItem.WebSiteId.HasValue)
            {
                dbItem.WebSiteKomisyon = await GetWebSiteYatirimKomisyon(dbItem.WebSiteId.Value, price);
            }

            dbItem.Tutar = price;
            dbItem.Toplam = dbItem.Tutar - dbItem.TedarikciKomisyon;
            dbItem.AdSoyadYatirimYapan = yatirimYapanAdSoyad.Trim();

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Kayıt güncellendi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var depositTransactionDbItem = await GetDepositTransactionAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (depositTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Deposit Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await GetWebSiteAsync(depositTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            yatirimTalepleriDbItem.Durum = Durum.Onaylandi;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Yatırım talebi onaylandı"
            };

            string depositCallbackUrl = webSiteItem.DepositCallbackUrl;
            if (yatirimTalepleriDbItem.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(yatirimTalepleriDbItem.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    depositCallbackUrl = webSitePaymentMethodItem.DepositCallbackUrl;
                }
            }

            try
            {
                var depositPostData = GenerateDepositCallBackDto(depositTransactionDbItem, yatirimTalepleriDbItem, Durum.Onaylandi, webSiteItem.Type);
                var content = await PostDepositCallbackAsync(depositPostData, depositCallbackUrl);

                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = content;

                depositTransactionDbItem.IsCompleted = true;

                if (yatirimTalepleriDbItem.HavaleBankaId.HasValue)
                {
                    var havaleBankalariItem = await _havaleBankalariRepository.GetByIdAsync(yatirimTalepleriDbItem.HavaleBankaId.Value);
                    havaleBankalariItem.OnaylananYatirimAdedi += 1;
                    havaleBankalariItem.OnaylananYatirimToplami += yatirimTalepleriDbItem.Tutar;
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                await _unitOfWork.SaveAsync();

                responseViewModel.Success = false;
                responseViewModel.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
            }

            return responseViewModel;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, string iptalAciklama)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var depositTransactionDbItem = await GetDepositTransactionAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (depositTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Deposit Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await GetWebSiteAsync(depositTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            yatirimTalepleriDbItem.IptalAciklama = iptalAciklama;
            yatirimTalepleriDbItem.Durum = Durum.Red;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            depositTransactionDbItem.IsCompleted = true;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Yatırım talebi iptal edildi",
            };

            return responseViewModel;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> ChangeStatus(Guid id, Durum durum)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            dbItem.Durum = durum;
            dbItem.OnayTarihi = DateTime.Now;

            var result = await _unitOfWork.SaveAsync();

            string message = string.Empty;
            if (durum == Durum.Red)
            {
                message = "Yatırım talebi iptal edildi";
            }
            else if (durum == Durum.Onaylandi)
            {
                message = "Yatırım talebi onaylandı";
            }

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = message,
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> Delete(Guid id)
        {
            var item = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum? durum, DateTime? approvedDateStart, DateTime? approvedDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 10)
        {
            var result = await _yatirimTalepleriRepository.GetTumYatirimTalepleriWithSpAsync(tedarikciId, bankaId, webSiteId, searchText, durum, approvedDateStart, approvedDateEnd, createdDateStart, createdDateEnd, orderBy, pageNumber, recordsPerPage);
            var count = result.Count;

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = result.YatirimTalepleriListDto.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetToplamYatirimTutari(tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamYatirimTutariDto> { Success = true, Result = result };
        }

        public async Task<ResponseViewModel<OnaySuresiDto>> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetOrtalamaOnaySuresi(tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<OnaySuresiDto> { Success = true, Result = result };
        }

        private string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private static string GenerateHashForPronet(string TransactionId, string UserId, int Status, string Type, string ClientId, string ClientSecret)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}{ClientId}{ClientSecret}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static bool IsApiliTedarikci(Settings settings, Guid tedarikciId)
        {
            var apiliTedarikciler = new[]
            {
                settings.FluxFinansTedarikciId,
                settings.ViPayTedarikciId,
                settings.ViPayTedarikciId2,
                settings.PullPayTedarikciId,
                settings.ParoPayTedarikciId,
                settings.ZenEntegreTedarikciId,
                settings.TumBankTedarikciId,
                settings.FixHavaleTedarikciId,
            };

            return tedarikciId != Guid.Empty && apiliTedarikciler.Contains(tedarikciId);
        }

        // Transactions projesindeki dahili endpoint üzerinden API'li tedarikçiye yatırım isteği atar
        private async Task<TedarikciHesapResponse> SendDepositToApiliTedarikciAsync(string webSiteTransactionId, Guid tedarikciId, decimal tutar)
        {
            var body = JsonSerializer.Serialize(new { webSiteTransactionId, tedarikciId, amount = tutar });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_transactionsApiSettings.Url.TrimEnd('/')}/api/internal/deposit/send-to-supplier");
            httpRequest.Headers.Add("X-Internal-Api-Key", _transactionsApiSettings.InternalApiKey);
            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            TedarikciHesapResponse? result = null;
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                result = JsonSerializer.Deserialize<TedarikciHesapResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (result == null)
            {
                return new TedarikciHesapResponse
                {
                    Success = false,
                    Message = $"Tedarikçiye istek gönderilemedi (HTTP {(int)response.StatusCode})."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                if (string.IsNullOrWhiteSpace(result.Message))
                {
                    result.Message = $"Tedarikçiye istek gönderilemedi (HTTP {(int)response.StatusCode}).";
                }
            }

            return result;
        }

        private async Task<decimal> GetTedarikciYatirimKomisyon(Guid tedarikciId, decimal tutar)
        {
            var tedarikciItem = await _tedarikciRepository.GetByIdAsync(tedarikciId);
            var yatirimKomisyonOrani = tedarikciItem.YaririmKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonOrani;
        }

        private async Task<decimal> GetWebSiteYatirimKomisyon(Guid webSiteId, decimal tutar)
        {
            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteId);
            var yatirimKomisyonOrani = webSiteItem.YaririmKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonOrani;
        }

        private YatirimCekimTalepleri? GetYatirimTalepleri(Guid id)
        {
            return _yatirimTalepleriRepository.Find(k => k.Id == id &&
                                                         k.IslemTipi == IslemTip.Deposit &&
                                                         k.Durum == Durum.Bekliyor &&
                                                         !k.SilindiMi).FirstOrDefault();
        }

        private async Task<DepositTransaction?> GetDepositTransactionAsync(string webSiteTransactionId)
        {
            return await _depositTransactionRepository.GetByTransactionIdAsync(webSiteTransactionId);
        }

        private async Task<WebSiteleri> GetWebSiteAsync(Guid webSiteId)
        {
            return await _webSiteleriRepository.GetByIdAsync(webSiteId);
        }

        private DepositCallBackDto GenerateDepositCallBackDto(DepositTransaction depositTransaction, YatirimCekimTalepleri yatirimCekimTalepleri, Durum durum, int websiteType)
        {
            var depositPostData = new DepositCallBackDto
            {
                userID = depositTransaction.UserId,
                username = depositTransaction.UserName,
                name = depositTransaction.FullName,
                amount = yatirimCekimTalepleri.Tutar,
                verifiedAmount = yatirimCekimTalepleri.Tutar,
                transaction_id = yatirimCekimTalepleri.WebSiteTransactionId,
                status = (int)durum, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = nameof(IslemTip.Deposit), //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = yatirimCekimTalepleri.YontemTipi
            };

            if (websiteType == (int)WebsiteType.Pronet)
            {
                depositPostData.hash = GenerateHashForPronet(depositPostData.transaction_id,
                                                             depositPostData.userID,
                                                             depositPostData.status,
                                                             depositPostData.type,
                                                             depositTransaction.ClientId,
                                                             depositTransaction.ClientSecret);
            }
            else
            {
                depositPostData.hash = GenerateHash(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);
            }


            return depositPostData;
        }

        private async Task<string> PostDepositCallbackAsync(DepositCallBackDto depositPostData, string depositCallbackUrl)
        {
            var serializerdItem = JsonSerializer.Serialize(depositPostData);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(depositCallbackUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
