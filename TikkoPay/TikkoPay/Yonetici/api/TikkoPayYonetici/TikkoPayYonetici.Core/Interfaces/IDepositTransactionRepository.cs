using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
