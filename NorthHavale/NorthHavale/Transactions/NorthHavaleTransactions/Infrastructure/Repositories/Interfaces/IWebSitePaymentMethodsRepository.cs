using NorthHavaleTransactions.Infrastructure.Dtos;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSitePaymentMethodsRepository
    {
        Task<WebSitePaymentMethodsDto> GetByClientId(string clientId);
        Task<WebSitePaymentMethodsDto> GetBySecretId(string clientId, string clientSecret);
    }
}
