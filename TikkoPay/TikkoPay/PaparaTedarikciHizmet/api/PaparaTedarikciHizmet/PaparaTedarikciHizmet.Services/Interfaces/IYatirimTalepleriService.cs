using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.YatirimTalepleri;
using PaparaTedarikciHizmet.Core.Enums;

namespace PaparaTedarikciHizmet.Services.Interfaces
{
    public interface IYatirimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> GetByIdAsync(Guid id);
        Task<ResponseViewModel<YatirimTalepleriListDto>> CreateAsync(YatirimTalepleriCreateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> UpdateAsync(YatirimTalepleriUpdateDto item);
        Task<ResponseViewModel<YatirimTalepleriListDto>> TedarikciyeYonlendirAsync(Guid id, Guid tedarikciId);
        Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePriceAsync(Guid id, decimal price);
        Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id, Guid tedarikciId);
        Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama);
        Task<ResponseViewModel<YatirimTalepleriListDto>> DeleteAsync(Guid id);
        Task<int> GetToplamBekleyenYatirimlar(Guid? tedarikciId);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
