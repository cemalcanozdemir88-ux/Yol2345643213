using PayCoYonetici.Core.DTOs.KullaniciYetkileri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
    }
}
