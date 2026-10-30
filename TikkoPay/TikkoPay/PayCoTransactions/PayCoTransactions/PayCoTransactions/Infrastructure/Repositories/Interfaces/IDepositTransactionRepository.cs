using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction> GetByTransactionId(string transactionId);
        Guid Insert(DepositCreateRequest request, Guid webSiteId);
        void UpdateIsCompleted(Guid id);
    }
}
