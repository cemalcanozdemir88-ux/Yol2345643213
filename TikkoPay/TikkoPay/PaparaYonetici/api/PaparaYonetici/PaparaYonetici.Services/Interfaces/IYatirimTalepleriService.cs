using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;
using PaparaYonetici.Core.Enums;

namespace PaparaYonetici.Services.Interfaces
{
    public interface IYatirimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Create(YatirimTalepleriCreateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Update(YatirimTalepleriUpdateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> TedarikciyeYonlendir(Guid id, Guid tedarikciId);
        Task<ResponseViewModel<YatirimTalepleriListDto>> BekleyeneAl(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePrice(Guid id, decimal price);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Approve(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> ChangeStatus(Guid id, Durum durum);
        Task<ResponseViewModel<YatirimTalepleriListDto>> Delete(Guid id);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
