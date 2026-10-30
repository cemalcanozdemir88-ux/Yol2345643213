using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;

namespace ParaYolCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IYatirimCekimTalepleriRepository
    {
        Task<int> GetCountByWebSiteTransactionId(string webSiteTransactionId);
        Task<YatirimCekimTalepleri> GetById(Guid id);
        Task<IEnumerable<YatirimCekimTalepleri>> GetCekimTalepleriByTopId(Guid topId);
        Task<YatirimCekimTalepleri> GetCekimTalepleriByTedarikciAndWebSiteTransactionId(Guid tedarikciId, string webSiteTransactionId);
        Task<YatirimCekimTalepleri> GetYatirimTalepleriByTedarikciAndWebSiteTransactionId(Guid tedarikciId, string webSiteTransactionId);
        Task<YatirimCekimTalepleri> GetByWebSiteTransactionId(Guid tedarikciId, string webSiteTransactionId);
        Task<YatirimCekimTalepleri> GetPendingYatirimTalepleri(string webSiteTransactionId);
        Task<YatirimCekimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId);
        Task UpdateDurumByIdAsync(Guid id, int durum);
        Task UpdateDurumByTransactionIdAsync(long transactionId, int durum);
        Task UpdateWebsiteCallBackResultByIdAsync(string id, string content);
        Task UpdateWebsiteCallBackResultByTransactionIdAsync(long transactionId, string content);
        Task UpdateWebsiteCallBackResultByTransactionIdWithTutarAsync(long transactionId, string content, decimal tutar, decimal tedarikciKomisyon, decimal webSiteKomisyon);
        Task UpdateReceiptByIdAsync(Guid id, string receipt);
    }
}
