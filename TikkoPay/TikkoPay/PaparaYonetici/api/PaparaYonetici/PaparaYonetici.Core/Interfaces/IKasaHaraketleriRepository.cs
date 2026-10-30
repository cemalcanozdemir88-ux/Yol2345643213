using PaparaYonetici.Core.DTOs.KasaHaraketleri;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IKasaHaraketleriRepository : IGenericRepository<KasaHaraketleri>
    {
        Task<KasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<int> GetCountAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
    }
}
