using ParaYolYonetici.Core.DTOs.WebSitePaymentMethods;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IWebSitePaymentMethodsRepository : IGenericRepository<WebSitePaymentMethods>
    {
        Task<IEnumerable<WebSitePaymentMethodsDto>> GetByWebSiteIdDtoAsync(Guid webSiteId);
    }
}
