using ParaYolCallbacks.Infrastructure.Entities;

namespace ParaYolCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IWebSiteRepository
    {
        Task<WebSiteleri> GetByIdAsync(Guid Id);
        Task<WebSiteleri> GetByIdWithAllColumnsAsync(Guid Id);
        Task<WebSiteleri> GetWebSiteItemAsync(string clientId, string clientSecret);
        Task<IEnumerable<WebSiteleri>> GetAllAsync();
        Task<decimal> GetYatirimKomisyonuAsync(Guid webSiteId, decimal tutar);
        Task<decimal> GetWebsiteBakiyeAsync(Guid websiteId);
    }
}
