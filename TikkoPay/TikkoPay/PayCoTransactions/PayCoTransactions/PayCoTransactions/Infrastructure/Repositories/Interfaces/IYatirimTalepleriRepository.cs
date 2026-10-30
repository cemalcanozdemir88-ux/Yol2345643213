using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IYatirimTalepleriRepository
    {
        Task<YatirimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId);
        Guid Insert(YatirimTalepleri yatirimCekimTalepleri);
        void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult);
    }
}
