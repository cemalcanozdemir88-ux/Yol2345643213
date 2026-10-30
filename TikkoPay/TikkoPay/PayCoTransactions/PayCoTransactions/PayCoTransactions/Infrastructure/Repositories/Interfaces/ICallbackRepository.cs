using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ICallbackRepository
    {
        Task<CallbackRequest> GetByCustomerTransactionId(string customerTransactionId);
        Task<int> Insert(PaycoCallbackRequest request);
    }
}
