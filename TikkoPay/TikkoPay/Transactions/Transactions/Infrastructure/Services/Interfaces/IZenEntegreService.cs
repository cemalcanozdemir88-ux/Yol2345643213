using Transactions.Infrastructure.Entities;
using Transactions.Models.ZenEntegre;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IZenEntegreService
    {
        Task<ZenEntegreBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
