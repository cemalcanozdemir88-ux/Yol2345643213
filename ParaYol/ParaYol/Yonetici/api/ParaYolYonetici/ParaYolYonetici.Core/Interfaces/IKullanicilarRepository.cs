using ParaYolYonetici.Core.DTOs.Kullanicilar;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<bool> IsUniqueEmail(string email);
        Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest);
    }
}
