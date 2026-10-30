using Transactions.Infrastructure.Entities;
using Transactions.Models.Nh;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface INhService
    {
        Task<NhBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
