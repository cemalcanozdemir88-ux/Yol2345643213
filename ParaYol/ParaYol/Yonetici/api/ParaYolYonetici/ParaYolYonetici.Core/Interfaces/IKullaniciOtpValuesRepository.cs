using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullaniciOtpValuesRepository
    {
        Task<KullaniciOtpValues> GetById(Guid Id);
        Task<Guid> Add(Guid KullaniciId);
    }
}
