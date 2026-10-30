using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Models;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IDepositRepository
    {
        Task<DepositTransaction> GetById(Guid Id);
        Task<DepositTransaction> GetByTransactionIdAsync(string transactionId);
        Task<int> GetPendingTransactionsCountByUserId(Guid WebSiteId, string UserId);
        Task<DepositTransaction> GetWaitingDepositTransaction(Guid webSiteId, string userId);
        Guid Insert(DepositCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto);
        Task<int> UpdateIsCompletedAsync(Guid Id, bool isCompleted);
    }
}
