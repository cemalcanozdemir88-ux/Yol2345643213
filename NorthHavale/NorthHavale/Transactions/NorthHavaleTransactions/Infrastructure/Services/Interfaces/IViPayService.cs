using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.ViPay;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IViPayService
    {
        Task<ViPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
