using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.FluxFinans;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IFluxFinansService
    {
        Task<FluxFinansBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
