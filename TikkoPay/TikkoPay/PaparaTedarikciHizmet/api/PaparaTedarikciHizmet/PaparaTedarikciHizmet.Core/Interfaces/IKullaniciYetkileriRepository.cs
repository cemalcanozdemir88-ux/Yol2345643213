using PaparaTedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
    }
}
