using TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSiteHavaleBankalariRepository : IGenericRepository<WebSiteHavaleBankalari>
    {
        Task<List<WebSiteHavaleBankalariDto>> GetByWebSiteIdAsync(Guid webSiteId);
        Task<bool> ExistsAsync(Guid webSiteId, Guid havaleBankaId);
    }
}
