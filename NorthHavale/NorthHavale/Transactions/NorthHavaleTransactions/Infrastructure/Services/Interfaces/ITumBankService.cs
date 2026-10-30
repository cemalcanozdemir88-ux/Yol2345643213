using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.TumBank;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface ITumBankService
    {
        Task<TumBankBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
