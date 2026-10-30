using ParaYolYonetici.Core.DTOs.WebSiteMutabakatlari;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWebSiteMutabakatRepository : IGenericRepository<WebSiteMutabakatlari>
    {
        Task<IEnumerable<WebSiteMutabakatlariListDto>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi);
        Task RemoveFromDb(Guid id);
    }
}
