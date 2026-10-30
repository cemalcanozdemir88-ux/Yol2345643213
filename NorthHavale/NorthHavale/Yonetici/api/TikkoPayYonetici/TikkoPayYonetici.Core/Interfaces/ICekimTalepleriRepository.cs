using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface ICekimTalepleriRepository : IGenericRepository<YatirimCekimTalepleri>
    {
        Task<CekimTalepleriListDto?> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<CekimTalepleriListDto>> GetByTopId(Guid? topId, bool showWebSite);
        Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<GetCekimTalepleriResponse> GetTumCekimTalepleriWithSpAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum durum, DateTime? approvedDateStart, DateTime? approvedDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 20);
        Task<IEnumerable<CekimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<OnaySuresiDto> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task DeleteByTopId(Guid topId);
    }
}
