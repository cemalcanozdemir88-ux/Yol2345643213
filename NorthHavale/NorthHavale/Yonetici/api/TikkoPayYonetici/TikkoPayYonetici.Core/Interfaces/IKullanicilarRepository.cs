using TikkoPayYonetici.Core.DTOs.Kullanicilar;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<bool> IsUniqueEmail(string email);
        Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest);
    }
}
