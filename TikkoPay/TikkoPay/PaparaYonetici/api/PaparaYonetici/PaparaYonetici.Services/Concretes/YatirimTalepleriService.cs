using AutoMapper;
using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.CallBacks;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Core.Models;
using PaparaYonetici.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace PaparaYonetici.Services.Concretes
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

        public YatirimTalepleriService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IYatirimTalepleriRepository yatirimTalepleriRepository,
            ITedarikciRepository tedarikciRepository,
            IWebSiteleriRepository webSiteleriRepository,
            IDepositTransactionRepository depositTransactionRepository,
            IHavaleBankalariRepository havaleBankalariRepository,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
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
            model.YontemTipi = "Manuel";

            if (model.IslemTipi == IslemTip.Deposit)
            {
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
            }

            if (model.IslemTipi == IslemTip.Withdraw)
            {
                model.HesapNo = item.HesapNo;
                model.HesapSahibi = item.HesapSahibi;

                if (model.TedarikciId.HasValue)
                {
                    model.TedarikciKomisyon = await GetTedarikciCekimKomisyon(model.TedarikciId.Value, model.Tutar);
                }

                if (model.WebSiteId.HasValue)
                {
                    model.WebSiteKomisyon = await GetWebSiteCekimKomisyon(model.WebSiteId.Value, model.Tutar);
                }
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

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> TedarikciyeYonlendir(Guid id, Guid tedarikciId)
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

            if (dbItem.Durum != Durum.Bekliyor || dbItem.IslemTipi != IslemTip.Withdraw)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Bekliyor durumundaki çekim talepleri yönlendirilebilir."
                };
            }

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? yonlendirenKullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            dbItem.YonlendirenKullaniciId = yonlendirenKullaniciId;
            dbItem.YonlendirilmeTarihi = DateTime.Now;
            dbItem.TedarikciKomisyon = await GetTedarikciCekimKomisyon(tedarikciId, dbItem.Tutar);
            dbItem.TedarikciId = tedarikciId;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Çekim talebi tedarikçiye yönlendirildi",
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

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePrice(Guid id, decimal price)
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

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Tutar güncellendi",
                Success = true
            };
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

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> Approve(Guid id)
        {
            var yatirimTalepleriDbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var depositTransactionDbItem = await _depositTransactionRepository.GetByTransactionIdAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (depositTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Deposit Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(depositTransactionDbItem.WebSiteId);
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

            var result = await _unitOfWork.SaveAsync();

            try
            {
                var depositPostData = new DepositCallBackDto();
                depositPostData.userID = depositTransactionDbItem.UserId;
                depositPostData.username = depositTransactionDbItem.UserName;
                depositPostData.name = depositTransactionDbItem.FullName;
                depositPostData.amount = yatirimTalepleriDbItem.Tutar;
                depositPostData.verifiedAmount = yatirimTalepleriDbItem.Tutar;
                depositPostData.transaction_id = yatirimTalepleriDbItem.WebSiteTransactionId;
                depositPostData.status = (int)Durum.Onaylandi; //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                depositPostData.type = nameof(IslemTip.Deposit); //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                depositPostData.payment_method_id = yatirimTalepleriDbItem.YontemTipi;
                depositPostData.hash = GenerateHash(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);

                var serializerdItem = JsonSerializer.Serialize(depositPostData);
                StringContent stringContent = new StringContent(serializerdItem, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webSiteItem.DepositCallbackUrl, stringContent);
                var statusCode = response.IsSuccessStatusCode;
                var content = await response.Content.ReadAsStringAsync();

                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = content;

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                await _unitOfWork.SaveAsync();

                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = $"İşlem sırasında hata oluştu: {ex.Message}"
                };
            }

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Yatırım talebi onaylandı",
                Success = true
            };
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

        public async Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText, islemTipi, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, webSiteId, searchText, islemTipi, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText, islemTipi, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, webSiteId, searchText, islemTipi, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText, islemTipi, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

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

        public async Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetToplamCekimTutari(tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamCekimTutariDto> { Success = true, Result = result };
        }

        private async Task<decimal> GetTedarikciCekimKomisyon(Guid tedarikciId, decimal tutar)
        {
            var tedariciItem = await _tedarikciRepository.GetByIdAsync(tedarikciId);
            var cekimKomisyonOrani = tedariciItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
        }

        private async Task<decimal> GetWebSiteCekimKomisyon(Guid webSiteId, decimal tutar)
        {
            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteId);
            var cekimKomisyonOrani = webSiteItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
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
    }
}
