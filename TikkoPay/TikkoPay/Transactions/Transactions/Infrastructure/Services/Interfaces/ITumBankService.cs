using Transactions.Infrastructure.Entities;
using Transactions.Models.TumBank;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface ITumBankService
    {
        Task<TumBankBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
