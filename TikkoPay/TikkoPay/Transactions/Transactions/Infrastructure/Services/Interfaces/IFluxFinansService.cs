using Transactions.Infrastructure.Entities;
using Transactions.Models.FluxFinans;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IFluxFinansService
    {
        Task<FluxFinansBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
