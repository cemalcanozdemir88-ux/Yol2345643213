using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileri;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IWebSiteTedarikcileriService
    {
        Task<ResponseViewModel<List<WebSiteTedarikcileriDto>>> GetByWebSiteId(Guid webSiteId);
        Task<ResponseViewModel<WebSiteTedarikcileriDto>> Create(WebSiteTedarikcileriCreateDto item);
        Task<ResponseViewModel<WebSiteTedarikcileriDto>> Delete(Guid id);
    }
}
