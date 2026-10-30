using PaparaTedarikciHizmet.Core.DTOs.Kullanicilar;
using PaparaTedarikciHizmet.Core.Enums;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IKullanicilarRepository : IGenericRepository<Kullanicilar>
    {
        Task<KullanicilarListDto> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi);
        Task<KullanicilarListDto> GetByEmailPassword(LoginRequest loginRequest);
    }
}
