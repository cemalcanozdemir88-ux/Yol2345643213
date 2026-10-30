using PayCoYonetici.Core.DTOs.KasaHaraketleri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IKasaHaraketleriRepository : IGenericRepository<KasaHaraketleri>
    {
        Task<KasaHaraketleriListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<int> GetCountAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        List<BakiyeListDto> GetBakiyeList();
    }
}
