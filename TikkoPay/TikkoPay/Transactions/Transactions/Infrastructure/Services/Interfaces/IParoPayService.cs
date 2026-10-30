using Transactions.Infrastructure.Entities;
using Transactions.Models.ParoPay;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IParoPayService
    {
        Task<ParoPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
