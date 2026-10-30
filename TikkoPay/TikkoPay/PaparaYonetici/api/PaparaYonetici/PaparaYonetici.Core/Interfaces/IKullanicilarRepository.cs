using PaparaYonetici.Core.DTOs.Kullanicilar;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<bool> IsUniqueEmail(string email);
        Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest);
    }
}
