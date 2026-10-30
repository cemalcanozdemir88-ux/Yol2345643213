using TikkoPayYonetici.Core.DTOs.KullaniciYetkileri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
        Task<bool> KullaniciYetkiVarMi(Guid kullaniciId, List<Guid> yetkiIds);
    }
}
