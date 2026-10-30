using System.Net.Http;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Options;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.HavaleBankalari;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class HavaleBankalariService : IHavaleBankalariService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly YoneticiPanelSettings _yoneticiPanelSettings;
        private readonly HttpClient _httpClient;
        private readonly IWebSiteTedarikcileriManualRepository _webSiteTedarikcileriManualRepository;

        public HavaleBankalariService(IMapper mapper,
                                      IUnitOfWork unitOfWork,
                                      IHavaleBankalariRepository havaleBankalariRepository,
                                      IOptions<YoneticiPanelSettings> yoneticiPanelSettingsOptions,
                                      HttpClient httpClient,
                                      IWebSiteTedarikcileriManualRepository webSiteTedarikcileriManualRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _havaleBankalariRepository = havaleBankalariRepository;
            _yoneticiPanelSettings = yoneticiPanelSettingsOptions.Value;
            _httpClient = httpClient;
            _webSiteTedarikcileriManualRepository = webSiteTedarikcileriManualRepository;
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> GetById(Guid id)
        {
            var item = await _havaleBankalariRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = true,
                    Result = _mapper.Map<HavaleBankalariListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<HavaleBankalariListDto>>> GetAll(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter)
        {
            var items = await _havaleBankalariRepository.GetAllDtoAsync(bankaId, searchText, havaleBankalariDurumFilter);
            return new ResponseViewModel<List<HavaleBankalariListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<HavaleBankalariListDto>>> GetAll(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage)
        {
            var count = await _havaleBankalariRepository.GetCountAsync(bankaId, searchText, havaleBankalariDurumFilter);
            var items = await _havaleBankalariRepository.GetAllDtoAsync(bankaId, searchText, havaleBankalariDurumFilter, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<HavaleBankalariListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Create(HavaleBankalariCreateDto item)
        {
            HavaleBankalari havaleBankalari = _mapper.Map<HavaleBankalari>(item);
            await _havaleBankalariRepository.AddAsync(havaleBankalari);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Update(HavaleBankalariUpdateDto item)
        {
            var dbItem = await _havaleBankalariRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var oldDurum = dbItem.Durum;
            var newDurum = item.Durum;

            item.HesapNo = dbItem.HesapNo; // hesap no değiştirilemez, herhangi bir açık olması durumunda hesapno bilgisinin değişmemesi için eklenmiştir.
            _havaleBankalariRepository.Update(dbItem, item);

            var result = await _unitOfWork.SaveAsync();
            if (result > 0)
            {
                if (oldDurum != newDurum)
                {
                    await SendNotification(dbItem);
                }
            }

            return new ResponseViewModel<HavaleBankalariListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Delete(Guid id)
        {
            var item = await _havaleBankalariRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            _havaleBankalariRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<HavaleBankalariListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        private async Task<string> SendNotification(HavaleBankalari model)
        {
            var webSiteTedarikci = await _webSiteTedarikcileriManualRepository.GetByTedarikciIdAsync(model.TedarikciId);
            foreach (var item in webSiteTedarikci)
            {
                var websiteId = item.WebSiteId.ToString();
                var serializerdItem = JsonSerializer.Serialize(websiteId);
                StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

                var postUrl = $"{_yoneticiPanelSettings.Url}/api/notify/update-havale-banka-list";
                var response = await _httpClient.PostAsync(postUrl, stringContent);
                var statusCode = response.IsSuccessStatusCode;
                var content = await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }
    }
}
