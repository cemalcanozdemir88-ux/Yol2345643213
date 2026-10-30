using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IKullaniciOtpValuesRepository
    {
        Task<KullaniciOtpValues> GetById(Guid Id);
        Task<Guid> Add(Guid KullaniciId);
    }
}
