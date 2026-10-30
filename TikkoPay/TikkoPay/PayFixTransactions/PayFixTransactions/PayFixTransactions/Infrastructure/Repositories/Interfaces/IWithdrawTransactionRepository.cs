using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction> GetByTransactionId(string transactionId);
        void CreateWithdraw(WithdrawCreateRequest request, WebSiteleri webSiteItem);
    }
}
