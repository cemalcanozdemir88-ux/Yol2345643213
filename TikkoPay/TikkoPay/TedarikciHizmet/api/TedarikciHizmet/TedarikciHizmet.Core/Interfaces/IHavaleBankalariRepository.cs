using TedarikciHizmet.Core.DTOs.HavaleBankalari;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IHavaleBankalariRepository : IGenericRepository<HavaleBankalari>
    {
        Task<HavaleBankalariListDto> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
    }
}
