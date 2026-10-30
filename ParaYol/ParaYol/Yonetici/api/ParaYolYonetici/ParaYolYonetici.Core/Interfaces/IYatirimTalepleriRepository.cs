using ParaYolYonetici.Core.DTOs.YatirimTalepleri;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IYatirimTalepleriRepository : IGenericRepository<YatirimCekimTalepleri>
    {
        Task<YatirimTalepleriListDto?> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetByTopId(Guid? topId, bool showWebSite);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<GetYatirimTalepleriResponse> GetTumYatirimTalepleriWithSpAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum? durum, DateTime? approvedDateStart, DateTime? approvedDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 20);
        Task<IEnumerable<YatirimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<OnaySuresiDto> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task DeleteByTopId(Guid topId);
    }
}
