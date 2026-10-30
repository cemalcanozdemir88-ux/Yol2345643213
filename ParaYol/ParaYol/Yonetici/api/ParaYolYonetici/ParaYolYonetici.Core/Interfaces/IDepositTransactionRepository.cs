using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
