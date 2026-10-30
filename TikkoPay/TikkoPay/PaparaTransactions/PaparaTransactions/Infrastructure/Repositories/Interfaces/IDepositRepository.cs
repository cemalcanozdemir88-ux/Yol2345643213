using Transactions.Infrastructure.Entities;
using Transactions.Models;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IDepositRepository
    {
        Task<DepositTransaction> GetById(Guid Id);
        Task<int> GetPendingTransactionsCountByUserId(Guid WebSiteId, string UserId);
        Task<DepositTransaction> GetWaitingDepositTransaction(Guid webSiteId, string userId);
        Guid Insert(DepositCreateRequest request, Guid webSiteId);
    }
}
