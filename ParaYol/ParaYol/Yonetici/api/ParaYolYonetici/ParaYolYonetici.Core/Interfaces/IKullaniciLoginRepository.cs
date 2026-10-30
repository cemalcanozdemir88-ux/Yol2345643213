using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullaniciLoginRepository
    {
        Task AddAsync(KullaniciLogin entity);
    }
}
