using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface ICekimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id, Guid tedarikciId, string onayAciklama);
        Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
    }
}
