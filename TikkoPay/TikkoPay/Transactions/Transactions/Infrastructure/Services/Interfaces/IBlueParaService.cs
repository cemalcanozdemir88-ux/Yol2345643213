using Transactions.Infrastructure.Entities;
using Transactions.Models.BluePara;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IBlueParaService
    {
        Task<BlueParaGenerateLinkResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
