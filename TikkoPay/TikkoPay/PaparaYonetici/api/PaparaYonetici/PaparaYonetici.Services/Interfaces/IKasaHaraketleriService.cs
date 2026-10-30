using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.KasaHaraketleri;

namespace PaparaYonetici.Services.Interfaces
{
    public interface IKasaHaraketleriService
    {
        Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id);
    }
}
