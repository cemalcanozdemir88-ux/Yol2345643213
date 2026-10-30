using PayFixYonetici.Core.DTOs.KullaniciYetkileri;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
    }
}
