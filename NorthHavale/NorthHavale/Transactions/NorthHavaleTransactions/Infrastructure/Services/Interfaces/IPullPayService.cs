using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.PullPay;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IPullPayService
    {
        Task<PullPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
