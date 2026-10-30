using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IDepositTransactionRepository
    {
        Task<DepositTransaction> GetByTransactionIdAsync(string transactionId);
    }
}
