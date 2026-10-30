using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IHavaleBankalariRepository
    {
        Task<HavaleBankalari> GetById(Guid id);
        Task<IEnumerable<Bankalar>> GetBankListForAmountOverFast(decimal amount);
        Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount, Guid webSiteId, string bankaId);
    }
}
