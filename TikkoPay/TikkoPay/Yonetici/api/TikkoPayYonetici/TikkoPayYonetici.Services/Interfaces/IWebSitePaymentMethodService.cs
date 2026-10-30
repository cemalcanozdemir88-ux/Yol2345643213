using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSitePaymentMethods;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSitePaymentMethodService
    {
        Task<ResponseViewModel<List<WebSitePaymentMethodsDto>>> GetByWebSiteIdDtoAsync(Guid webSiteId);
    }
}
