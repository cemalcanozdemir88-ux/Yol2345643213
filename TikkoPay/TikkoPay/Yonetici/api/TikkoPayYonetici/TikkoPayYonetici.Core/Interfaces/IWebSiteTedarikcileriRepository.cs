using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSiteTedarikcileriRepository : IGenericRepository<WebSiteTedarikcileri>
    {
        Task<List<WebSiteTedarikcileriDto>> GetByWebSiteIdAsync(Guid webSiteId);
        Task<List<WebSiteTedarikcileriDto>> GetByTedarikciIdAsync(Guid tedarikciId);
    }
}
