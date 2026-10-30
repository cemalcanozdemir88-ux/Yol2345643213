using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using TikkoPayYonetici.Core;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.CallBacks;
using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class CekimTalepleriService : ICekimTalepleriService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly ICekimTalepleriRepository _cekimTalepleriRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly IFluxFinansService _fluxFinansService;
        private readonly IViPayService _viPayService;
        private readonly IPullPayService _pullPayService;
        private readonly IParoPayService _paroPayService;
        private readonly IZenService _zenService;
        private readonly ITumBankService _tumBankService;
        private readonly IFixHavaleService _fixHavaleService;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public CekimTalepleriService(IMapper mapper,
            IUnitOfWork unitOfWork,
            ICekimTalepleriRepository cekimTalepleriRepository,
            ITedarikciRepository tedarikciRepository,
            IWebSiteleriRepository webSiteleriRepository,
            IWithdrawTransactionRepository withdrawTransactionRepository,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository,
            IFluxFinansService fluxFinansService,
            IViPayService viPayService,
            IPullPayService pullPayService,
            IParoPayService paroPayService,
            IZenService zenService,
            ITumBankService tumBankService,
            IFixHavaleService fixHavaleService,
            ISettingsV2Repository settingsV2Repository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cekimTalepleriRepository = cekimTalepleriRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
            _fluxFinansService = fluxFinansService;
            _viPayService = viPayService;
            _pullPayService = pullPayService;
            _paroPayService = paroPayService;
            _zenService = zenService;
            _tumBankService = tumBankService;
            _fixHavaleService = fixHavaleService;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> GetByIdAsync(Guid id)
        {
            var item = await _cekimTalepleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<CekimTalepleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<GetDekontDto>> GetDekontByIdAsync(Guid id)
        {
            var item = await _cekimTalepleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<GetDekontDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<GetDekontDto>
                {
                    Success = true,
                    Result = new GetDekontDto
                    {
                        Id = item.Id,
                        DekontKontrolDurumu = item.DekontKontrolDurumu ?? 0,
                        DekontKontrolKullanici = item.DekontKontrolKullanici,
                        DekontKontrolTarihi = item.DekontKontrolTarihi,
                        Info = item.IptalAciklama ?? string.Empty
                    }
                };
            }
        }

        public async Task<ResponseViewModel<List<CekimTalepleriListDto>>> GetByTopIdAsync(Guid? topId, bool showWebSite)
        {
            var items = await _cekimTalepleriRepository.GetByTopId(topId, showWebSite);
            return new ResponseViewModel<List<CekimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModel<List<CekimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _cekimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<CekimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<CekimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, DekontKontrolDurum? dekontKontrolDurum, Durum? durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 10)
        {
            var result = await _cekimTalepleriRepository.GetTumCekimTalepleriWithSpAsync(tedarikciId,
                bankaId,
                webSiteId,
                searchText,
                dekontKontrolDurum,
                durum,
                approveDateStart,
                approveDateEnd,
                createdDateStart,
                createdDateEnd,
                orderBy,
                pageNumber,
                recordsPerPage);

            var count = result.Count;

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<CekimTalepleriListDto>>
            {
                Success = true,
                Result = result.CekimTalepleriListDto.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _cekimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _cekimTalepleriRepository.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutariAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _cekimTalepleriRepository.GetToplamCekimTutari(tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamCekimTutariDto> { Success = true, Result = result };
        }

        public async Task<ResponseViewModel<OnaySuresiDto>> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _cekimTalepleriRepository.GetOrtalamaOnaySuresi(tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<OnaySuresiDto> { Success = true, Result = result };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> CreateAsync(CekimTalepleriCreateDto item)
        {
            YatirimCekimTalepleri model = _mapper.Map<YatirimCekimTalepleri>(item);

            model.TalepNo = DateTime.Now.ToString("yyyyMMdd");
            model.PaymentMethodId = Constants.TikkoPaymentMethodId;
            model.YontemTipi = "Manuel";
            model.IslemTipi = IslemTip.Withdraw;
            model.HesapNo = item.HesapNo;
            model.HesapSahibi = item.HesapSahibi;

            if (model.TedarikciId.HasValue)
            {
                model.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(model.TedarikciId.Value, model.Tutar);
            }

            if (model.WebSiteId.HasValue)
            {
                model.WebSiteKomisyon = await GetWebSiteCekimKomisyonAsync(model.WebSiteId.Value, model.Tutar);
            }

            model.Toplam = model.Tutar - model.TedarikciKomisyon;
            //model.Durum = Durum.Onaylandi;
            model.SilindiMi = false;

            if (model.Durum == Durum.Onaylandi || model.Durum == Durum.Red)
            {
                model.OnayTarihi = DateTime.Now;
                model.GuncellemeTarihi = DateTime.Now;
            }

            model.KayitTarihi = DateTime.Now;


            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            model.GuncelleyenKullaniciId = kullaniciId;

            await _cekimTalepleriRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = true,
                    Result = new CekimTalepleriListDto { Id = model.Id },
                };
            }
            else
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> UpdateAsync(CekimTalepleriUpdateDto itemDto)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _cekimTalepleriRepository.Update(dbItem, itemDto);
            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> UpdateDekontKontrol(UpdateDekontKontrolDto request)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(request.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            dbItem.DekontKontrolDurumu = request.DekontKontrolDurumu;
            dbItem.DekontKontrolKullaniciId = kullaniciId;
            dbItem.DekontKontrolTarihi = DateTime.Now;

            var result = await _unitOfWork.SaveAsync(setBaseModelValues: false);

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Dekont Kontrol Durumu Güncellendi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> UpdateBekleyenCekim(UpdateBekleyenCekimDto request)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(request.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Bekliyor)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Sadece bekleyen çekim güncellenebilir"
                };
            }

            var isUserIdInt = long.TryParse(request.UserId, out long userIdBigIntValue);

            dbItem.UserId = request.UserId;
            dbItem.UserIdBigIntValue = isUserIdInt ? userIdBigIntValue : null;
            dbItem.UserName = request.UserName;
            dbItem.AdSoyad = request.AdSoyad;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim Güncellendi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> TedarikciyeYonlendirAsync(Guid id, Guid tedarikciId)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Bekliyor || dbItem.IslemTipi != IslemTip.Withdraw)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Bekliyor durumundaki çekim talepleri yönlendirilebilir."
                };
            }

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? yonlendirenKullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            dbItem.YonlendirenKullaniciId = yonlendirenKullaniciId;
            dbItem.YonlendirilmeTarihi = DateTime.Now;
            dbItem.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(tedarikciId, dbItem.Tutar);
            dbItem.TedarikciId = tedarikciId;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim talebi tedarikçiye yönlendirildi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> MultiTedarikciyeYonlendirAsync(MultiTedarikciYonlendirmeDto request)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(request.CekimTalepleriId);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Bekliyor || dbItem.IslemTipi != IslemTip.Withdraw)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Bekliyor durumundaki çekim talepleri yönlendirilebilir."
                };
            }

            if (request.TedarikciWithAmount.Count == 0)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Yönlendirilecek tedarikçi seçilmedi."
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? yonlendirenKullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            await _cekimTalepleriRepository.DeleteByTopIdExceptApproved(dbItem.Id);

            if (request.TedarikciWithAmount.Count == 1)
            {
                var tedarikciId = request.TedarikciWithAmount[0].TedarikciId;

                dbItem.YonlendirenKullaniciId = yonlendirenKullaniciId;
                dbItem.YonlendirilmeTarihi = DateTime.Now;
                dbItem.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(tedarikciId, dbItem.Tutar);
                dbItem.TedarikciId = tedarikciId;

                dbItem.CokluTedarikciyeYonlendirildi = false;
                dbItem.TopId = null;

                if (tedarikciId.Equals(settings.FluxFinansTedarikciId))
                {
                    var serviceResponse = await _fluxFinansService.SendWithdrawToFluxFinanceAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.ViPayTedarikciId))
                {
                    var serviceResponse = await _viPayService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.PullPayTedarikciId))
                {
                    var serviceResponse = await _pullPayService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.ParoPayTedarikciId))
                {
                    var serviceResponse = await _paroPayService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.ZenEntegreTedarikciId))
                {
                    var serviceResponse = await _zenService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.TumBankTedarikciId))
                {
                    var serviceResponse = await _tumBankService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
                else if (tedarikciId.Equals(settings.FixHavaleTedarikciId))
                {
                    var serviceResponse = await _fixHavaleService.SendWithdrawAsync(dbItem);
                    if (!serviceResponse.IsSuccess)
                    {
                        return new ResponseViewModel<CekimTalepleriListDto>
                        {
                            Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                            Success = false
                        };
                    }
                }
            }
            else
            {
                dbItem.TedarikciId = null;
                dbItem.CokluTedarikciyeYonlendirildi = true;

                dbItem.YonlendirenKullaniciId = yonlendirenKullaniciId;
                dbItem.YonlendirilmeTarihi = DateTime.Now;

                var childItems = await _cekimTalepleriRepository.GetByTopId(dbItem.Id, true);

                var approvedChildIds = childItems.Where(k => k.Durum == Durum.Onaylandi)
                                                 .Select(k => k.TedarikciId)
                                                 .ToHashSet();

                foreach (var tedarikciWithAmountItem in request.TedarikciWithAmount.Where(x => !approvedChildIds.Contains(x.TedarikciId)))
                {
                    var newItem = dbItem.DeepClone();
                    newItem.Id = Guid.NewGuid();
                    newItem.TopId = dbItem.Id;
                    newItem.TedarikciId = tedarikciWithAmountItem.TedarikciId;
                    newItem.YonlendirenKullaniciId = yonlendirenKullaniciId;
                    newItem.YonlendirilmeTarihi = DateTime.Now;
                    newItem.Tutar = tedarikciWithAmountItem.Amount;
                    newItem.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(tedarikciWithAmountItem.TedarikciId, tedarikciWithAmountItem.Amount);
                    newItem.Toplam = newItem.Tutar - newItem.TedarikciKomisyon;
                    newItem.CokluTedarikciyeYonlendirildi = false;

                    if (tedarikciWithAmountItem.TedarikciId.Equals(settings.FluxFinansTedarikciId))
                    {
                        var serviceResponse = await _fluxFinansService.SendWithdrawToFluxFinanceAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.ViPayTedarikciId))
                    {
                        var serviceResponse = await _viPayService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.PullPayTedarikciId))
                    {
                        var serviceResponse = await _pullPayService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.ParoPayTedarikciId))
                    {
                        var serviceResponse = await _paroPayService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.ZenEntegreTedarikciId))
                    {
                        var serviceResponse = await _zenService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.TumBankTedarikciId))
                    {
                        var serviceResponse = await _tumBankService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }
                    else if (tedarikciWithAmountItem.TedarikciId.Equals(settings.FixHavaleTedarikciId))
                    {
                        var serviceResponse = await _fixHavaleService.SendWithdrawAsync(newItem);
                        if (!serviceResponse.IsSuccess)
                        {
                            return new ResponseViewModel<CekimTalepleriListDto>
                            {
                                Message = $"Çekim talebi tedarikçiye yönlendirilirken hata oluştu! {serviceResponse.Message}",
                                Success = false
                            };
                        }
                    }

                    await _cekimTalepleriRepository.AddAsync(newItem);
                }

            }

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim talebi tedarikçiye yönlendirildi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> ApproveAsync(Guid id)
        {
            var cekimTalepleriDbItem = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (cekimTalepleriDbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepleriDbItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Withdraw Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(withDrawTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            cekimTalepleriDbItem.IptalAciklama = $"{kullaniciId} kullanıcısı onayladı";
            cekimTalepleriDbItem.Durum = Durum.Onaylandi;
            cekimTalepleriDbItem.OnayTarihi = DateTime.Now;

            var result = await _unitOfWork.SaveAsync();

            string withdrawCallbackUrl = webSiteItem.WithdrawCallbackUrl;
            if (cekimTalepleriDbItem.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(cekimTalepleriDbItem.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    withdrawCallbackUrl = webSitePaymentMethodItem.WithdrawCallbackUrl;
                }
            }

            try
            {
                var withdrawalPostData = GenerateWithdrawalCallBackDto(withDrawTransactionDbItem, cekimTalepleriDbItem, Durum.Onaylandi, webSiteItem.Type);
                var content = await PostWithdrawalCallbackAsync(withdrawalPostData, withdrawCallbackUrl);

                cekimTalepleriDbItem.IsSendToWebSite = true;
                cekimTalepleriDbItem.WebSiteCallBackResult = content;

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                cekimTalepleriDbItem.IsSendToWebSite = true;
                cekimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                await _unitOfWork.SaveAsync();

                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = $"İşlem sırasında hata oluştu: {ex.Message}"
                };
            }

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim talebi onaylandı",
                Success = true
            };

        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> TedarikciSil(Guid id)
        {
            var cekimTalepleriDbItem = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (cekimTalepleriDbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            cekimTalepleriDbItem.TedarikciId = null;
            cekimTalepleriDbItem.TedarikciKomisyon = 0;
            cekimTalepleriDbItem.YonlendirilmeTarihi = null;
            cekimTalepleriDbItem.YonlendirenKullaniciId = null;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim talebinden tedarikçi silindi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> CancelAsync(Guid id)
        {
            var cekimTalepleriDbItem = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (cekimTalepleriDbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepleriDbItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Withdraw Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(withDrawTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

            cekimTalepleriDbItem.IptalAciklama = $"{kullaniciId} kullanıcısı iptal etti";
            cekimTalepleriDbItem.Durum = Durum.Red;
            cekimTalepleriDbItem.OnayTarihi = DateTime.Now;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = "Çekim talebi iptal edildi",
                Success = true
            };

        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> DeleteAsync(Guid id)
        {
            var item = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<ResponseViewModel<CekimTalepleriListDto>> ChangeStatusAsync(Guid id, Durum durum)
        {
            var dbItem = await _cekimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<CekimTalepleriListDto>
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
                message = "Çekim talebi iptal edildi";
            }
            else if (durum == Durum.Onaylandi)
            {
                message = "Çekim talebi onaylandı";
            }

            return new ResponseViewModel<CekimTalepleriListDto>
            {
                Message = message,
                Success = true
            };
        }

        private WithdrawalCallBackDto GenerateWithdrawalCallBackDto(WithdrawTransaction withdrawTransaction, YatirimCekimTalepleri cekimTalepleri, Durum durum, int websiteType)
        {
            var withdrawalPostData = new WithdrawalCallBackDto
            {
                userID = withdrawTransaction.UserId,
                username = withdrawTransaction.UserName,
                name = withdrawTransaction.FullName,
                amount = cekimTalepleri.Tutar,
                verifiedAmount = cekimTalepleri.Tutar,
                transaction_id = cekimTalepleri.WebSiteTransactionId,
                status = (int)durum, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = "Withdrawal", //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = cekimTalepleri.YontemTipi
            };

            if (websiteType == (int)WebsiteType.Pronet)
            {
                withdrawalPostData.hash = GenerateHashForPronet(withdrawalPostData.transaction_id,
                                                                withdrawalPostData.userID,
                                                                withdrawalPostData.status,
                                                                withdrawalPostData.type,
                                                                withdrawTransaction.ClientId,
                                                                withdrawTransaction.ClientSecret);
            }
            else
            {
                withdrawalPostData.hash = GenerateHash(withdrawalPostData.transaction_id, withdrawalPostData.userID, withdrawalPostData.status, withdrawalPostData.type);
            }

            return withdrawalPostData;
        }

        private async Task<string> PostWithdrawalCallbackAsync(WithdrawalCallBackDto withdrawalPostData, string WithdrawCallbackUrl)
        {
            var serializerdItem = JsonSerializer.Serialize(withdrawalPostData);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(WithdrawCallbackUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
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

        private async Task<decimal> GetTedarikciCekimKomisyonAsync(Guid tedarikciId, decimal tutar)
        {
            var tedariciItem = await _tedarikciRepository.GetByIdAsync(tedarikciId);
            var cekimKomisyonOrani = tedariciItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
        }

        private async Task<decimal> GetWebSiteCekimKomisyonAsync(Guid webSiteId, decimal tutar)
        {
            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteId);
            var cekimKomisyonOrani = webSiteItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
        }

    }
}
