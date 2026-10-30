using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction?> GetByTransactionIdAsync(string transactionId);
    }
}
