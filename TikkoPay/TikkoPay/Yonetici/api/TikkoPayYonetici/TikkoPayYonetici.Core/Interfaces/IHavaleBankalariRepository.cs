using TikkoPayYonetici.Core.DTOs.HavaleBankalari;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IHavaleBankalariRepository : IGenericRepository<HavaleBankalari>
    {
        Task<HavaleBankalariListDto?> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoByTedarikciIdListAsync(List<Guid> tedarikciIds);
    }
}
