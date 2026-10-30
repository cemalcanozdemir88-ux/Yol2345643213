using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileri;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSiteTedarikcileriService
    {
        Task<ResponseViewModel<List<WebSiteTedarikcileriDto>>> GetByWebSiteId(Guid webSiteId);
        Task<ResponseViewModel<List<WebSiteTedarikcileriDto>>> GetByTedarikciId(Guid tedarikciId);
        Task<ResponseViewModel<WebSiteTedarikcileriDto>> Create(WebSiteTedarikcileriCreateDto item);
        Task<ResponseViewModel<WebSiteTedarikcileriDto>> Delete(Guid id);
    }
}
