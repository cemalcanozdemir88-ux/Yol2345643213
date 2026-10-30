using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction> GetByTransactionId(string transactionId);
        void CreateWithdraw(WithdrawCreateRequest request, WebSiteleri webSiteItem);
    }
}
