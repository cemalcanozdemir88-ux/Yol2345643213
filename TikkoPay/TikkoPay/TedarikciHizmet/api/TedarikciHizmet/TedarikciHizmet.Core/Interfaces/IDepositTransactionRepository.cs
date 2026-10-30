using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction> GetByTransactionIdAsync(string transactionId);
    }
}
