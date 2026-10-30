using TikkoPayYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSiteKasaHaraketleriRepository : IGenericRepository<WebSiteKasaHaraketleri>
    {
        Task<WebSiteKasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<int> GetCountAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
