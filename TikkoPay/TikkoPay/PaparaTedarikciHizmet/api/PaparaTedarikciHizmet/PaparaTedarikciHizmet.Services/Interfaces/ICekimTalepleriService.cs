using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.YatirimTalepleri;
using PaparaTedarikciHizmet.Core.Enums;

namespace PaparaTedarikciHizmet.Services.Interfaces
{
    public interface ICekimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id, Guid tedarikciId, string onayAciklama);
        Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama);
        Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
    }
}
