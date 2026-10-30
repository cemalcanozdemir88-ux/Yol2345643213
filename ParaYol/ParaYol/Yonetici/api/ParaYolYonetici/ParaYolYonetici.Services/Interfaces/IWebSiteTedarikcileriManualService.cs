using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileriManual;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IWebSiteTedarikcileriManualService
    {
        Task<ResponseViewModel<List<WebSiteTedarikcileriManualDto>>> GetByWebSiteId(Guid webSiteId);
        Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Create(WebSiteTedarikcileriManualCreateDto item);
        Task<ResponseViewModel<WebSiteTedarikcileriManualDto>> Delete(Guid id);
    }
}
