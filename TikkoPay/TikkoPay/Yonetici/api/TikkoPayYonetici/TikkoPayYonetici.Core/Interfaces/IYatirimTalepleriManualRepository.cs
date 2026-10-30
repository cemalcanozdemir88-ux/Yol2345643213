using TikkoPayYonetici.Core.DTOs.YatirimTalepleriManual;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IYatirimTalepleriManualRepository : IGenericRepository<YatirimCekimTalepleriManual>
    {
        Task<IEnumerable<YatirimTalepleriManualBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<YatirimTalepleriManualListDto> GetByIdWitnRelations(Guid id);
        Task<int> GetCountAsync(Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
