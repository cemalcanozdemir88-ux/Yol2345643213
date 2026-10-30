using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
