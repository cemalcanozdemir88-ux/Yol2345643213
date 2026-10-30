using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.YatirimTalepleri;
using ParaYolYonetici.Core.DTOs.YatirimTalepleriManual;
using ParaYolYonetici.Core.Enums;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IYatirimTalepleriManuelService
    {
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> BekleyeneAl(Guid id);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> ChangeIsSendToWebSiteStatus(Guid id);
        Task<ResponseViewModel<YatirimTalepleriManualBaseListDto>> Create(Guid webSiteId, YatirimTalepleriManuelCreateDto item);
        Task<ResponseViewModelPagination<List<YatirimTalepleriManualBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> GetById(Guid id);
    }
}
