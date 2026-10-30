using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSiteTedarikcileriManualService
    {
        Task<ResponseViewModel<List<WebSiteTedarikcileriManualDto>>> GetByWebSiteId(Guid webSiteId);
        Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Create(WebSiteTedarikcileriManualCreateDto item);
        Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Delete(Guid id);
    }
}
