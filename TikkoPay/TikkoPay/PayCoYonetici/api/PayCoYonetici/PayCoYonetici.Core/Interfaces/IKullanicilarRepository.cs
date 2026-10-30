using PayCoYonetici.Core.DTOs.Kullanicilar;
using PayCoYonetici.Core.Enums;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<bool> IsUniqueEmail(string email);
        Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest);
    }
}
