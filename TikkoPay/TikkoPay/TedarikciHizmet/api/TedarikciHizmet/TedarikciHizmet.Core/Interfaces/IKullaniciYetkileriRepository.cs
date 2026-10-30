using TedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
    }
}
