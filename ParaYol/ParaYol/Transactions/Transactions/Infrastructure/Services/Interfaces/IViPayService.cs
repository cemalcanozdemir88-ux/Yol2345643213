using Transactions.Infrastructure.Entities;
using Transactions.Models.ViPay;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IViPayService
    {
        Task<ViPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
