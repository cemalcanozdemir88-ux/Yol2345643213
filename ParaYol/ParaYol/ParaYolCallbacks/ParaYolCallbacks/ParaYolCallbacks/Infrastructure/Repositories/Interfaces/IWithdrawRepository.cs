using ParaYolCallbacks.Infrastructure.Entities;

namespace ParaYolCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        Task<WithdrawTransaction> GetById(Guid Id);
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
    }
}
