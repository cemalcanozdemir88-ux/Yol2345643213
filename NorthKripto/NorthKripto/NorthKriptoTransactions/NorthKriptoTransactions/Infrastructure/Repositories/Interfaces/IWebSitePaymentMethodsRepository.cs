using NorthKriptoTransactions.Infrastructure.Dtos;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWebSitePaymentMethodsRepository
    {
        Task<WebSitePaymentMethodsDto> GetBySecretId(string clientId, string clientSecret);
    }
}
