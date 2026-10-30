using ParaYolYonetici.Core.DTOs.TedarikciMutabakatlari;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface ITedarikciMutabakatRepository : IGenericRepository<TedarikciMutabakatlari>
    {
        Task<IEnumerable<TedarikciMutabakatlariListDto>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi);
        Task RemoveFromDb(Guid id);
    }
}
