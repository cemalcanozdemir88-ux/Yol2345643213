using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IWithdrawTransactionRepository
    {
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
    }
}
