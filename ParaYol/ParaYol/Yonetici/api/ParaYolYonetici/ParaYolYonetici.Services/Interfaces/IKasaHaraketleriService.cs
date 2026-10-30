using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.KasaHaraketleri;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IKasaHaraketleriService
    {
        Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<KasaHaraketleriListDto>> CreateVirman(KasaHaraketleriVirmanCreateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id);
    }
}
