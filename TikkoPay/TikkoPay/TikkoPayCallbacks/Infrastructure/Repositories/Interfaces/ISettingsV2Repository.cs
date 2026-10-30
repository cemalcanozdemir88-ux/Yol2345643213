using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface ISettingsV2Repository
    {
        Task<Settings> GetSettingsAsync();
    }
}
