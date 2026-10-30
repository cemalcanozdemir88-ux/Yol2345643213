using PayFixYonetici.Core.DTOs;
using PayFixYonetici.Core.DTOs.YatirimTalepleri;
using PayFixYonetici.Core.Enums;

namespace PayFixYonetici.Services.Interfaces
{
    public interface IYatirimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
