using Transactions.Infrastructure.Dtos;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSitePaymentMethodsRepository
    {
        Task<WebSitePaymentMethodsDto> GetBySecretId(string clientId, string clientSecret);
    }
}
