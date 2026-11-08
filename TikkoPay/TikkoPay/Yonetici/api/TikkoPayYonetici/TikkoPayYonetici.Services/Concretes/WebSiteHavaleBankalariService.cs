using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class WebSiteHavaleBankalariService : IWebSiteHavaleBankalariService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteHavaleBankalariRepository _webSiteHavaleBankalariRepository;

        public WebSiteHavaleBankalariService(IUnitOfWork unitOfWork,
                                             IWebSiteHavaleBankalariRepository webSiteHavaleBankalariRepository)
        {
            _unitOfWork = unitOfWork;
            _webSiteHavaleBankalariRepository = webSiteHavaleBankalariRepository;
        }

        public async Task<ResponseViewModel<List<WebSiteHavaleBankalariDto>>> GetByWebSiteId(Guid webSiteId)
        {
            var items = await _webSiteHavaleBankalariRepository.GetByWebSiteIdAsync(webSiteId);
            return new ResponseViewModel<List<WebSiteHavaleBankalariDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<WebSiteHavaleBankalariDto>> Create(WebSiteHavaleBankalariCreateDto item)
        {
            if (item.WebSiteId == Guid.Empty || item.HavaleBankaId == Guid.Empty)
            {
                return new ResponseViewModel<WebSiteHavaleBankalariDto>
                {
                    Success = false,
                    Message = "Web sitesi ve hesap seçilmelidir."
                };
            }

            if (await _webSiteHavaleBankalariRepository.ExistsAsync(item.WebSiteId, item.HavaleBankaId))
            {
                return new ResponseViewModel<WebSiteHavaleBankalariDto>
                {
                    Success = false,
                    Message = "Bu hesap bu web sitesine zaten bağlı."
                };
            }

            await _webSiteHavaleBankalariRepository.AddAsync(new WebSiteHavaleBankalari
            {
                WebSiteId = item.WebSiteId,
                HavaleBankaId = item.HavaleBankaId
            });

            var result = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteHavaleBankalariDto>
            {
                Success = result > 0,
                Message = result > 0 ? "Hesap web sitesine bağlandı." : "Hata oluştu, bir süre sonra tekrar deneyiniz."
            };
        }

        public async Task<ResponseViewModel<WebSiteHavaleBankalariDto>> Delete(Guid id)
        {
            var item = await _webSiteHavaleBankalariRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteHavaleBankalariDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteHavaleBankalariDto>
            {
                Success = true,
                Message = "Hesap bağlantısı silindi"
            };
        }
    }
}
