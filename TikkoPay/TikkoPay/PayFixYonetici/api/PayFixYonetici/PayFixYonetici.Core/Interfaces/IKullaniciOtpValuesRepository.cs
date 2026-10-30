using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IKullaniciOtpValuesRepository
    {
        Task<KullaniciOtpValues> GetById(Guid Id);
        Task<Guid> Add(Guid KullaniciId);
    }
}
