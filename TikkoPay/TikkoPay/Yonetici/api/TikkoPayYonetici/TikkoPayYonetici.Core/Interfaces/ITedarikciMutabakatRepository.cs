using TikkoPayYonetici.Core.DTOs.TedarikciMutabakatlari;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface ITedarikciMutabakatRepository : IGenericRepository<TedarikciMutabakatlari>
    {
        Task<IEnumerable<TedarikciMutabakatlariListDto>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi);
        Task RemoveFromDb(Guid id);
    }
}
