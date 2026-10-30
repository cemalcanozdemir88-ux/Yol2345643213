using Transactions.Infrastructure.Entities;
using Transactions.Models.PullPay;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IPullPayService
    {
        Task<PullPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
