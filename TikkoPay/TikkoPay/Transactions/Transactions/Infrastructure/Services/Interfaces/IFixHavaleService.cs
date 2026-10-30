using Transactions.Infrastructure.Entities;
using Transactions.Models.FixHavale;

namespace Transactions.Infrastructure.Services.Interfaces
{
    public interface IFixHavaleService
    {
        Task<FixApiDepositResponseDto> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount);
    }
}
