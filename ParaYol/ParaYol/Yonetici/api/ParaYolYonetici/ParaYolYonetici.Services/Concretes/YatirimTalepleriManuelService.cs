using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Options;
using ParaYolYonetici.Core;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.YatirimTalepleri;
using ParaYolYonetici.Core.DTOs.YatirimTalepleriManual;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class YatirimTalepleriManuelService : IYatirimTalepleriManuelService
    {
        private readonly IMapper _mapper;
        private readonly IYatirimTalepleriManualRepository _yatirimTalepleriManualRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly TedarikciPanelSettings _tedarikciPanelSettings;

        public YatirimTalepleriManuelService(IMapper mapper,
                                             IYatirimTalepleriManualRepository yatirimTalepleriManualRepository,
                                             IHavaleBankalariRepository havaleBankalariRepository,
                                             ITedarikciRepository tedarikciRepository,
                                             IWebSiteleriRepository webSiteleriRepository,
                                             IUnitOfWork unitOfWork,
                                             HttpClient httpClient,
                                             IOptions<TedarikciPanelSettings> tedarikciPanelSettingsOptions)
        {
            _mapper = mapper;
            _yatirimTalepleriManualRepository = yatirimTalepleriManualRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _tedarikciPanelSettings = tedarikciPanelSettingsOptions.Value;
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> GetById(Guid id)
        {
            var item = await _yatirimTalepleriManualRepository.GetByIdWitnRelations(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = true,
                    Result = _mapper.Map<YatirimTalepleriManualListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> BekleyeneAl(Guid id)
        {
            var dbItem = await _yatirimTalepleriManualRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Red || dbItem.IslemTipi != IslemTip.Deposit)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "İptal edilen yatırımlar bekleyene alınabilir."
                };
            }

            dbItem.Durum = Durum.Bekliyor;

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Message = "Yatırım talebi bekleyene alındı",
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> ChangeIsSendToWebSiteStatus(Guid id)
        {
            var dbItem = await _yatirimTalepleriManualRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            dbItem.IsSendToWebSite = !dbItem.IsSendToWebSite;
            if (dbItem.IsSendToWebSite)
            {
                dbItem.WebSiteCallBackResult = "Bakiye üye hesabına yansıtıldı";
            }
            else
            {
                dbItem.WebSiteCallBackResult = "Bakiye üye hesabına yansıtılMAdı";
            }

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Message = "İşlem başarılı",
                Success = true
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriManualBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriManualRepository.GetCountAsync(webSiteId, searchText, showWebSite, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriManualRepository.GetAllForWebSiteAsync(webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriManualBaseListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualBaseListDto>> Create(Guid webSiteId, YatirimTalepleriManuelCreateDto item)
        {
            var havaleBankalariItem = await _havaleBankalariRepository.GetByIdAsync(item.HavaleBankaId);
            if (havaleBankalariItem is null)
            {
                return new ResponseViewModel<YatirimTalepleriManualBaseListDto>
                {
                    Success = false,
                    Message = "Havale banka bulunamadı"
                };
            }

            if (!havaleBankalariItem.Durum)
            {
                return new ResponseViewModel<YatirimTalepleriManualBaseListDto>
                {
                    Success = false,
                    Message = "Havale banka aktif değil"
                };
            }

            YatirimCekimTalepleriManual model = new YatirimCekimTalepleriManual();
            model.HavaleBankaId = havaleBankalariItem.Id;
            model.TedarikciId = havaleBankalariItem.TedarikciId;
            model.WebSiteId = webSiteId;
            model.AdSoyad = item.AdSoyad;
            model.UserName = item.UserName;
            model.UserId = item.UserId;
            model.Tutar = item.Tutar;
            model.Durum = Durum.Bekliyor;
            model.TalepNo = DateTime.Now.ToString("yyyyMMdd");
            model.PaymentMethodId = Constants.ParaYolPaymentMethodId;
            model.YontemTipi = "Manuel";
            model.IslemTipi = IslemTip.Deposit;

            var userIdConvertResult = long.TryParse(model.UserId?.Trim(), out long userIdBigIntValue);
            if (userIdConvertResult)
            {
                model.UserIdBigIntValue = userIdBigIntValue;
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
            model.SilindiMi = false;
            model.KayitTarihi = DateTime.Now;

            await _yatirimTalepleriManualRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                var dbItem = await _yatirimTalepleriManualRepository.GetByIdWitnRelations(model.Id);
                await SendNotification(dbItem);

                return new ResponseViewModel<YatirimTalepleriManualBaseListDto>
                {
                    Success = true,
                    Result = new YatirimTalepleriManualBaseListDto { Id = model.Id },
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriManualBaseListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
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

        private async Task<string> SendNotification(YatirimTalepleriManualListDto model)
        {
            var serializerdItem = JsonSerializer.Serialize(model);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var postUrl = $"{_tedarikciPanelSettings.Url}/api/notify/send-yatirim-talebi";

            var response = await _httpClient.PostAsync(postUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
