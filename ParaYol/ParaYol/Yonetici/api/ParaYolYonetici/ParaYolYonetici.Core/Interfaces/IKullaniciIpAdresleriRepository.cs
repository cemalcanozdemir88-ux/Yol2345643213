using ParaYolYonetici.Core.DTOs.KullaniciIpAdresleri;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullaniciIpAdresleriRepository : IGenericRepository<KullaniciIpAdresleri>
    {
        Task<KullaniciIpAdresleriListDto?> GetByIdDtoAsync(Guid id);
        Task<List<KullaniciIpAdresleri>> GetByKullaniciIdAsync(Guid kullaniciId);
        Task<IEnumerable<KullaniciIpAdresleriListDto>> GetAllDtoAsync(Guid? tedarikciId, string searchText);
    }
}
