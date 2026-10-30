using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ICekimTalepleriRepository
    {
        Task<CekimTalepleri> GetByTransactionIdAsync(string webSiteTransactionId);
        Task<CekimTalepleriStatusCheckDto> GetStatusCheckDtoAsync(Guid webSiteId, string webSiteTransactionId);
    }
}
