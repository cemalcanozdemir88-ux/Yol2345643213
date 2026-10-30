using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        Task<WithdrawTransaction> GetById(Guid Id);
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
    }
}
