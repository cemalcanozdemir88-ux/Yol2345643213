using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction> GetByTransactionId(string transactionId);
        Guid Insert(DepositCreateRequest request, Guid webSiteId);
        void UpdateIsCompleted(Guid id);
    }
}
