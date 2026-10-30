using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface ISettingsRepository
    {
        Task<Settings> GetFirst();
    }
}
