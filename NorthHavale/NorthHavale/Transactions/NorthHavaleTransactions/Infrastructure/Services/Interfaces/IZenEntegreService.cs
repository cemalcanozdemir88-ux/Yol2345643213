using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.ZenEntegre;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IZenEntegreService
    {
        Task<ZenEntegreBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
