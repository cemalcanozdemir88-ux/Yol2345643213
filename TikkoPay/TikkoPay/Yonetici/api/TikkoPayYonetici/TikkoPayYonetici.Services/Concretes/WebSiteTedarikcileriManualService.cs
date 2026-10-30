using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class WebSiteTedarikcileriManualService : IWebSiteTedarikcileriManualService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteTedarikcileriManualRepository _webSiteTedarikcileriManualRepository;

        public WebSiteTedarikcileriManualService(IMapper mapper,
                                           IUnitOfWork unitOfWork,
                                           IWebSiteTedarikcileriManualRepository webSiteTedarikcileriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteTedarikcileriManualRepository = webSiteTedarikcileriRepository;
        }

        public async Task<ResponseViewModel<List<WebSiteTedarikcileriManualDto>>> GetByWebSiteId(Guid webSiteId)
        {
            var items = await _webSiteTedarikcileriManualRepository.GetByWebSiteIdAsync(webSiteId);
            return new ResponseViewModel<List<WebSiteTedarikcileriManualDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Create(WebSiteTedarikcileriManualCreateDto item)
        {
            var model = new WebSiteTedarikcileriManual
            {
                WebSiteId = item.WebSiteId,
                TedarikciId = item.TedarikciId
            };

            await _webSiteTedarikcileriManualRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<WebSiteTedarikcileriManualDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteTedarikcileriManualDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Delete(Guid id)
        {
            var item = await _webSiteTedarikcileriManualRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteTedarikcileriManualDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteTedarikcileriManualDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
