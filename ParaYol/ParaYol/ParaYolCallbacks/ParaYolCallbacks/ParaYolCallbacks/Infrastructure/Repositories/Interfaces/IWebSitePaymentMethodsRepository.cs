using ParaYolCallbacks.Infrastructure.Entities;

namespace ParaYolCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface IWebSitePaymentMethodsRepository
    {
        Task<WebSitePaymentMethods> GetByIdAsync(Guid id);
    }
}
