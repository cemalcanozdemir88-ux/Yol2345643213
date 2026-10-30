using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSitePaymentMethods;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IWebSitePaymentMethodService
    {
        Task<ResponseViewModel<List<WebSitePaymentMethodsDto>>> GetByWebSiteIdDtoAsync(Guid webSiteId);
    }
}
