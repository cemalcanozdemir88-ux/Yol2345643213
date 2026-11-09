using TikkoPayYonetici.Core.DTOs.WebSiteMutabakatlari;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSiteMutabakatRepository : IGenericRepository<WebSiteMutabakatlari>
    {
        Task<IEnumerable<WebSiteMutabakatlariListDto>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi);
        Task RemoveFromDb(Guid id);
        Task<bool> ReplaceForDateAsync(WebSiteMutabakatlari entity);
    }
}
