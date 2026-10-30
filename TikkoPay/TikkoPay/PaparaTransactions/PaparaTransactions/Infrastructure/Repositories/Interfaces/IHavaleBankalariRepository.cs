using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IHavaleBankalariRepository
    {
        Task<HavaleBankalari> GetById(Guid id);
        Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount);
    }
}
