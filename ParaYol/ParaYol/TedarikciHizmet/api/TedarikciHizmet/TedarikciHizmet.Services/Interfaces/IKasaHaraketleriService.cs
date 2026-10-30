using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.KasaHaraketleri;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface IKasaHaraketleriService
    {
        Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id);
        Task<decimal> GetToplamKasaHaraketiGiris(Guid tedarikciId);
        Task<decimal> GetToplamKasaHaraketiCikis(Guid tedarikciId);
    }
}
