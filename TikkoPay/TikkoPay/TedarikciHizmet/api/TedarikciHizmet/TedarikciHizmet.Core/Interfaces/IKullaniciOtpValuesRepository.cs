using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IKullaniciOtpValuesRepository
    {
        Task<KullaniciOtpValues> GetById(Guid Id);
        Task<Guid> Add(Guid KullaniciId);
    }
}
