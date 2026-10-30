using PayFixYonetici.Core.DTOs;
using PayFixYonetici.Core.DTOs.CekimTalepleri;

namespace PayFixYonetici.Services.Interfaces
{
    public interface ICekimTalepleriService
    {
        Task<ResponseViewModelPagination<List<CekimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20);
        Task<ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20);
        Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
