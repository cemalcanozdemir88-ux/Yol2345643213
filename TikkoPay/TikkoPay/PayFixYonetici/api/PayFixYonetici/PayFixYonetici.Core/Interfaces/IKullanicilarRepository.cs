using PayFixYonetici.Core.DTOs.Kullanicilar;
using PayFixYonetici.Core.Enums;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi, Guid? webSiteId, Guid? affiliateId);
        Task<bool> IsUniqueEmail(string email);
        Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest);
    }
}
