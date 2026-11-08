using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSiteHavaleBankalariService
    {
        Task<ResponseViewModel<List<WebSiteHavaleBankalariDto>>> GetByWebSiteId(Guid webSiteId);
        Task<ResponseViewModel<WebSiteHavaleBankalariDto>> Create(WebSiteHavaleBankalariCreateDto item);
        Task<ResponseViewModel<WebSiteHavaleBankalariDto>> Delete(Guid id);
    }
}
