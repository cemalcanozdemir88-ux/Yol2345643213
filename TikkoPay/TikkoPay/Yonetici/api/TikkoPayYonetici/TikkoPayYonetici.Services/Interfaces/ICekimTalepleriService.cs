using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface ICekimTalepleriService
    {
        Task<ResponseViewModel<CekimTalepleriListDto>> GetByIdAsync(Guid id);
        Task<ResponseViewModel<GetDekontDto>> GetDekontByIdAsync(Guid id);
        Task<ResponseViewModel<List<CekimTalepleriListDto>>> GetByTopIdAsync(Guid? topId, bool showWebSite);
        Task<ResponseViewModel<List<CekimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<CekimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, DekontKontrolDurum? dekontKontrolDurum, Durum? durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int orderBy = 1, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutariAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<OnaySuresiDto>> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<CekimTalepleriListDto>> CreateAsync(CekimTalepleriCreateDto item);
        Task<ResponseViewModel<CekimTalepleriListDto>> UpdateAsync(CekimTalepleriUpdateDto itemDto);
        Task<ResponseViewModel<CekimTalepleriListDto>> TedarikciyeYonlendirAsync(Guid id, Guid tedarikciId);
        Task<ResponseViewModel<CekimTalepleriListDto>> MultiTedarikciyeYonlendirAsync(MultiTedarikciYonlendirmeDto request);
        Task<ResponseViewModel<CekimTalepleriListDto>> ApproveAsync(Guid id);
        Task<ResponseViewModel<CekimTalepleriListDto>> CancelAsync(Guid id);
        Task<ResponseViewModel<CekimTalepleriListDto>> DeleteAsync(Guid id);
        Task<ResponseViewModel<CekimTalepleriListDto>> ChangeStatusAsync(Guid id, Durum durum);
        Task<ResponseViewModel<CekimTalepleriListDto>> UpdateDekontKontrol(UpdateDekontKontrolDto request);
        Task<ResponseViewModel<CekimTalepleriListDto>> UpdateBekleyenCekim(UpdateBekleyenCekimDto request);
        Task<ResponseViewModel<CekimTalepleriListDto>> TedarikciSil(Guid id);
    }
}
