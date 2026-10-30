using PayCoYonetici.Core.DTOs.CekimTalepleri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface ICekimTalepleriRepository : IGenericRepository<CekimTalepleri>
    {
        Task<int> GetCountAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20);
        Task<IEnumerable<CekimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20);
        Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
