using PaparaTedarikciHizmet.Core.DTOs.Tedarikciler;
using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface ITedarikciRepository : IGenericRepository<Tedarikciler>
    {
        Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId);
    }
}
