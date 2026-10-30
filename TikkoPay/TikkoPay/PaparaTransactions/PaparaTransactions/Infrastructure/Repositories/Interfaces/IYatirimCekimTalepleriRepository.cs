using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IYatirimCekimTalepleriRepository
    {
        Task<int> GetCountByWebSiteTransactionId(string webSiteTransactionId);
        Task<YatirimCekimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId);
        void Insert(YatirimCekimTalepleri yatirimCekimTalepleri);
    }
}
