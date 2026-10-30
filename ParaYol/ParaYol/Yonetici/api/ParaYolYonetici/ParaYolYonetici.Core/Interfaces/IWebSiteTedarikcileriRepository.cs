using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileri;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWebSiteTedarikcileriRepository : IGenericRepository<WebSiteTedarikcileri>
    {
        Task<List<WebSiteTedarikcileriDto>> GetByWebSiteIdAsync(Guid webSiteId);
    }
}
