using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSiteTedarikcileriManualRepository : IGenericRepository<WebSiteTedarikcileriManual>
    {
        Task<List<WebSiteTedarikcileriManualDto>> GetByWebSiteIdAsync(Guid webSiteId);
    }
}
