using TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKullaniciIpAdresleriRepository : IGenericRepository<KullaniciIpAdresleri>
    {
        Task<KullaniciIpAdresleriListDto?> GetByIdDtoAsync(Guid id);
        Task<List<KullaniciIpAdresleri>> GetByKullaniciIdAsync(Guid kullaniciId);
        Task<IEnumerable<KullaniciIpAdresleriListDto>> GetAllDtoAsync(Guid? tedarikciId, string searchText);
    }
}
