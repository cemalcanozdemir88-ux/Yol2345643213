using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IBankaKodlariRepository
    {
        Task<BankaKodlari> GetByKodAsync(string kod);
    }
}
