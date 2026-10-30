using ParaYolYonetici.Core.DTOs.KullaniciYetkileri;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullaniciYetkileriRepository : IGenericRepository<KullaniciYetkileri>
    {
        Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId);
        Task<bool> KullaniciYetkiVarMi(Guid kullaniciId, List<Guid> yetkiIds);
    }
}
