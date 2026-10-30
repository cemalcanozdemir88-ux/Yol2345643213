using PayFixYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IWebSiteKasaHaraketleriRepository : IGenericRepository<WebSiteKasaHaraketleri>
    {
        Task<WebSiteKasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
