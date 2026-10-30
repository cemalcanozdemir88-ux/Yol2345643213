using ParaYolYonetici.Core.DTOs.HavaleBankalari;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
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
