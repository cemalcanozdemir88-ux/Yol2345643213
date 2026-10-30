using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface IYatirimTalepleriManualService
    {
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> GetByIdAsync(Guid id);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> UpdateAsync(YatirimTalepleriManualUpdateDto item);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> ApproveAsync(Guid id, Guid tedarikciId);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama);
        Task<ResponseViewModel<YatirimTalepleriManualListDto>> DeleteAsync(Guid id);
        Task<int> GetToplamBekleyenYatirimlar(Guid? tedarikciId);
        Task<ResponseViewModel<List<YatirimTalepleriManualListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriManualListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);

    }
}
