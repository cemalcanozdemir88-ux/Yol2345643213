using PayFixYonetici.Core.DTOs.YatirimTalepleri;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IYatirimTalepleriRepository : IGenericRepository<YatirimTalepleri>
    {
        Task<YatirimTalepleriListDto?> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<IEnumerable<YatirimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
