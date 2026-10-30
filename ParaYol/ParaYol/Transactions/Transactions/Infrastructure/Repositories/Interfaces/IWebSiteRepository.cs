using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSiteRepository
    {
        Task<WebSiteleri> GetByIdAsync(Guid Id);
        Task<WebSiteleri> GetByIdWithAllColumnsAsync(Guid Id);
        Task<WebSiteleri> GetWebSiteItemAsync(string clientId, string clientSecret);
        Task<decimal> GetYatirimKomisyonuAsync(Guid webSiteId, decimal tutar);
        Task<decimal> GetWebsiteBakiyeAsync(Guid websiteId);
    }
}
