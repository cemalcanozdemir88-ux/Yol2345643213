using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IYatirimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Create(YatirimTalepleriCreateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Update(YatirimTalepleriUpdateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> BekleyeneAl(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> YenidenYonlendir(YenidenYonlendirRequest request);
        Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePriceAndYatirimYapanAdSoyad(Guid id, decimal price, string yatirimYapanAdSoyad);
        Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, string iptalAciklama);
        Task<ResponseViewModel<YatirimTalepleriListDto>> ChangeStatus(Guid id, Durum durum);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Delete(Guid id);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum? durum, DateTime? approvedDateStart, DateTime? approvedDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<OnaySuresiDto>> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
