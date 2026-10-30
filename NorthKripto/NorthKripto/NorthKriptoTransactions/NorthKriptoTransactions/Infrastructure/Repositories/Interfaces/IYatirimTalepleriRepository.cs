using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IYatirimTalepleriRepository
    {
        Task<YatirimTalepleri> GetByTransactionIdAsync(string webSiteTransactionId);
        Task<YatirimTalepleriStatusCheckDto> GetStatusCheckDtoAsync(Guid webSiteId, string webSiteTransactionId);
    }
}
