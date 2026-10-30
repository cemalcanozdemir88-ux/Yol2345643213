using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ICallbackRepository
    {
        Task<CallbackRequest> GetByCustomerTransactionId(string customerTransactionId);
        Task<int> Insert(PayFixCallbackRequest request);
    }
}
