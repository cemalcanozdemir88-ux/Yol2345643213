using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.ParoPay;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IParoPayService
    {
        Task<ParoPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
