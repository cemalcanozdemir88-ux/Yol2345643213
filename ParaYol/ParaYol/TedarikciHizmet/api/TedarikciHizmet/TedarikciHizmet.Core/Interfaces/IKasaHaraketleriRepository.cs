using TedarikciHizmet.Core.DTOs.KasaHaraketleri;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IKasaHaraketleriRepository : IGenericRepository<KasaHaraketleri>
    {
        Task<KasaHaraketleriListDto> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<int> GetCountAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<decimal> GetToplamKasaHaraketiGiris(Guid tedarikciId);
        Task<decimal> GetToplamKasaHaraketiCikis(Guid tedarikciId);
    }
}
