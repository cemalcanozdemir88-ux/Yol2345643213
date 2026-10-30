using TedarikciHizmet.Core.DTOs.TedarikciMutabakatlari;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface ITedarikciMutabakatRepository : IGenericRepository<TedarikciMutabakatlari>
    {
        Task<IEnumerable<TedarikciMutabakatlariListDto>> GetAllDtoAsync(Guid tedarikciId, DateOnly mutabakatTarihi);
    }
}
