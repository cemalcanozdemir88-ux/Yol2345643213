using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
