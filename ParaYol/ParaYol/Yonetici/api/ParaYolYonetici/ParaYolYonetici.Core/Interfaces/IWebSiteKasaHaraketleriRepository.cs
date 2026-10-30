using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWebSiteKasaHaraketleriRepository : IGenericRepository<WebSiteKasaHaraketleri>
    {
        Task<WebSiteKasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<int> GetCountAsync(Guid? webSiteId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task DeleteByTopId(Guid topId);
    }
}
