using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface ISettingsV2Repository
    {
        Task<IEnumerable<SettingsV2>> GetAllAsync();
        Task<Settings> GetSettingsAsync();
        Task<decimal> GetMaxFastTutar();
    }
}
