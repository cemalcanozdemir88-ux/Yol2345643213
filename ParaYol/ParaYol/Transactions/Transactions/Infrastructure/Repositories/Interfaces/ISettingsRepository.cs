using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<Settings> GetFirst();
        Task<decimal> GetMaxFastTutar();
        Task<Guid> GetAsPayDefaultFastHavaleBankaIdAsync();
        Task SaveAsPayTokenAsync(Guid id, string token, DateTime expireDate);
    }
}
