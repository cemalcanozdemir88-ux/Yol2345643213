using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IKullaniciOtpValuesRepository
    {
        Task<KullaniciOtpValues> GetById(Guid Id);
        Task<Guid> Add(Guid KullaniciId);
    }
}
