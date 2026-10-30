using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.KasaHaraketleri;

namespace PayCoYonetici.Services.Interfaces
{
    public interface IKasaHaraketleriService
    {
        Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<List<BakiyeListDto>>> GetBakiyeList();
        Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id);
    }
}
