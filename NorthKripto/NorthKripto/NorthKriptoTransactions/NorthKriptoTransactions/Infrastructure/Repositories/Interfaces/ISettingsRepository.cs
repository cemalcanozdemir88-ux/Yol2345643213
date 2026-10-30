using NorthKriptoTransactions.Infrastructure.Entities;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<Settings> GetFirst();
    }
}
