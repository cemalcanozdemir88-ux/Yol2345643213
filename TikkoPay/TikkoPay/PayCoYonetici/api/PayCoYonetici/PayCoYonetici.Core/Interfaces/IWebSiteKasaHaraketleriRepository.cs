using PayCoYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IWebSiteKasaHaraketleriRepository : IGenericRepository<WebSiteKasaHaraketleri>
    {
        Task<WebSiteKasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
