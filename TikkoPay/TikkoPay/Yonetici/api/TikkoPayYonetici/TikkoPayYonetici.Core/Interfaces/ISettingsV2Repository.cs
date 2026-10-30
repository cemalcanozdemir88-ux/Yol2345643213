using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface ISettingsV2Repository
    {
        Task<Settings> GetSettingsAsync();
    }
}
