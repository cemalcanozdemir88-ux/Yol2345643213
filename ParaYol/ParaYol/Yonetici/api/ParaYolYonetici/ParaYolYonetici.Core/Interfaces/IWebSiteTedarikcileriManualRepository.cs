using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileriManual;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWebSiteTedarikcileriManualRepository : IGenericRepository<WebSiteTedarikcileriManual>
    {
        Task<List<WebSiteTedarikcileriManualDto>> GetByWebSiteIdAsync(Guid webSiteId);
    }
}
