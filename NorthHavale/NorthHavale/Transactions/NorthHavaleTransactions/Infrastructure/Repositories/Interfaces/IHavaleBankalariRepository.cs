using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IHavaleBankalariRepository
    {
        Task<HavaleBankalari> GetById(Guid id);
        Task<IEnumerable<Bankalar>> GetBankListForAmountOverFast(decimal amount);
        Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount, Guid webSiteId, string bankaId);
    }
}
