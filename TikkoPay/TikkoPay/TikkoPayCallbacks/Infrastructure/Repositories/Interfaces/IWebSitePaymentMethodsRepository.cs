using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IWebSitePaymentMethodsRepository
    {
        Task<WebSitePaymentMethods> GetByIdAsync(Guid id);
    }
}
