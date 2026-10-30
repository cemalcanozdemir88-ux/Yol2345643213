using PaparaYonetici.Core.DTOs.Tedarikciler;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface ITedarikciRepository : IGenericRepository<Tedarikciler>
    {
        Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId);
    }
}
