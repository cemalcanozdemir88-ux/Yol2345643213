using TedarikciHizmet.Core.DTOs.Tedarikciler;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface ITedarikciRepository : IGenericRepository<Tedarikciler>
    {
        Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId);
    }
}
