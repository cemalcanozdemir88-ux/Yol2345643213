using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKullaniciLoginRepository
    {
        Task AddAsync(KullaniciLogin entity);
    }
}
