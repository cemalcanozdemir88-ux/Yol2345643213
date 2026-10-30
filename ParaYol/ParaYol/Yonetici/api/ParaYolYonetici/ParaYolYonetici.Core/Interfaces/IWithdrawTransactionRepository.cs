using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
