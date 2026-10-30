using ParaYolCallbacks.Infrastructure.Entities;

namespace ParaYolCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<Settings> GetFirst();
        Task<decimal> GetMaxFastTutar();
        Task<Guid> GetAsPayDefaultFastHavaleBankaIdAsync();
        Task SaveAsPayTokenAsync(Guid id, string token, DateTime expireDate);
    }
}
