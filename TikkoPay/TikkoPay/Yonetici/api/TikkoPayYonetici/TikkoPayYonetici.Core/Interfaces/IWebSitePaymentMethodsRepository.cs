using TikkoPayYonetici.Core.DTOs.WebSitePaymentMethods;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IWebSitePaymentMethodsRepository : IGenericRepository<WebSitePaymentMethods>
    {
        Task<IEnumerable<WebSitePaymentMethodsDto>> GetByWebSiteIdDtoAsync(Guid webSiteId);
    }
}
