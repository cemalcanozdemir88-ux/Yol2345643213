using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models.FixHavale;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IFixHavaleService
    {
        Task<FixApiDepositResponseDto> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
