using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.BluePara;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IBlueParaService
    {
        Task<BlueParaGenerateLinkResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
