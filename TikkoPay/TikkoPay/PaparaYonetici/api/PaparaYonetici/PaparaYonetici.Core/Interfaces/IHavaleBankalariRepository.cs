using PaparaYonetici.Core.DTOs.HavaleBankalari;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IHavaleBankalariRepository : IGenericRepository<HavaleBankalari>
    {
        Task<HavaleBankalariListDto?> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
    }
}
