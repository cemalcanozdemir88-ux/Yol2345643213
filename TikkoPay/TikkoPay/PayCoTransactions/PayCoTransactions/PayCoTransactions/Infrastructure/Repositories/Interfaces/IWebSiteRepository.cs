using PayCoTransactions.Infrastructure.Entities;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSiteRepository
    {
        Task<WebSiteleri> GetById(Guid webSiteId);
        Task<WebSiteleri> GetWebSiteItem(string clientId, string clientSecret);
    }
}
