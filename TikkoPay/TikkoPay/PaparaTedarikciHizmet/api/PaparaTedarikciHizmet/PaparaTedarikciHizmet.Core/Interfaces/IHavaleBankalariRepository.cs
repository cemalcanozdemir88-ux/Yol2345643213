using PaparaTedarikciHizmet.Core.DTOs.HavaleBankalari;
using PaparaTedarikciHizmet.Core.Enums;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IHavaleBankalariRepository : IGenericRepository<HavaleBankalari>
    {
        Task<HavaleBankalariListDto> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
    }
}
