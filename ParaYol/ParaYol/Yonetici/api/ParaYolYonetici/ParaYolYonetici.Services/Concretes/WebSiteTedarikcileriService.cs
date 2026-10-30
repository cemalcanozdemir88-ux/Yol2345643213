using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteleri;
using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileri;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class WebSiteTedarikcileriService : IWebSiteTedarikcileriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteTedarikcileriRepository _webSiteTedarikcileriRepository;

        public WebSiteTedarikcileriService(IMapper mapper,
                                           IUnitOfWork unitOfWork,
                                           IWebSiteTedarikcileriRepository webSiteTedarikcileriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteTedarikcileriRepository = webSiteTedarikcileriRepository;
        }

        public async Task<ResponseViewModel<List<WebSiteTedarikcileriDto>>> GetByWebSiteId(Guid webSiteId)
        {
            var items = await _webSiteTedarikcileriRepository.GetByWebSiteIdAsync(webSiteId);
            return new ResponseViewModel<List<WebSiteTedarikcileriDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<WebSiteTedarikcileriDto>> Create(WebSiteTedarikcileriCreateDto item)
        {
            var model = new WebSiteTedarikcileri
            {
                WebSiteId = item.WebSiteId,
                TedarikciId = item.TedarikciId
            };

            await _webSiteTedarikcileriRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<WebSiteTedarikcileriDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteTedarikcileriDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<WebSiteTedarikcileriDto>> Delete(Guid id)
        {
            var item = await _webSiteTedarikcileriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteTedarikcileriDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteTedarikcileriDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
