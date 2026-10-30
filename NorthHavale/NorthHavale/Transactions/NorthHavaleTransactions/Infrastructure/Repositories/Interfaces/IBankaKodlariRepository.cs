using NorthHavaleTransactions.Infrastructure.Entities;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IBankaKodlariRepository
    {
        Task<BankaKodlari> GetByKodAsync(string kod);
    }
}
