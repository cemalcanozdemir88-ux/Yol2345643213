using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSiteRepository
    {
        Task<WebSiteleri> GetById(Guid Id);
        Task<WebSiteleri> GetWebSiteItem(string clientId, string clientSecret);
        Task<decimal> GetYatirimKomisyonu(Guid webSiteId, decimal tutar);
    }
}
