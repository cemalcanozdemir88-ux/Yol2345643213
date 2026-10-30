using ParaYolYonetici.Core.DTOs.YatirimTalepleriManual;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IYatirimTalepleriManualRepository : IGenericRepository<YatirimCekimTalepleriManual>
    {
        Task<IEnumerable<YatirimTalepleriManualBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<YatirimTalepleriManualListDto> GetByIdWitnRelations(Guid id);
        Task<int> GetCountAsync(Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
