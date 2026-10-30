using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IYatirimTalepleriRepository
    {
        Task<YatirimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId);
        Guid Insert(YatirimTalepleri yatirimCekimTalepleri);
        void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult);
    }
}
