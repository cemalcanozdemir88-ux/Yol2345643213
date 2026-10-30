using TikkoPayYonetici.Core.DTOs.KasaHaraketleri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKasaHaraketleriRepository : IGenericRepository<KasaHaraketleri>
    {
        Task<KasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<int> GetCountAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task DeleteByTopId(Guid topId);
    }
}
