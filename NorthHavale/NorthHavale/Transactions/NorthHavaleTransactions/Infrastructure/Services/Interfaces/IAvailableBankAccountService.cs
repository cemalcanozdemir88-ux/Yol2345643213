using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;

namespace NorthHavaleTransactions.Infrastructure.Services.Interfaces
{
    public interface IAvailableBankAccountService
    {
        Task<AvailableHavaleBankItemDto?> GetValidBankAccountAsync(DepositTransaction depositTransaction, WebSiteleri webSiteItem, decimal amount, string bankaId);
    }
}
