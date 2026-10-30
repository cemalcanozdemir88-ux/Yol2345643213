using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IDepositRepository
    {
        Task<DepositTransaction> GetById(Guid Id);
        Task<DepositTransaction> GetByTransactionIdAsync(string transactionId);
        Task<int> GetPendingTransactionsCountByUserId(Guid WebSiteId, string UserId);
        Task<DepositTransaction> GetWaitingDepositTransaction(Guid webSiteId, string userId);
        Task<int> UpdateIsCompletedAsync(Guid Id, bool isCompleted);
    }
}
