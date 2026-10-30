using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<KullanicilarListDto> GetByEmailPassword(LoginRequest loginRequest);
    }
}
