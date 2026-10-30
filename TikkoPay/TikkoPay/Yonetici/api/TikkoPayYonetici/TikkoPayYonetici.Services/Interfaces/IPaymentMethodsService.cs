using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.PaymentMethods;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IPaymentMethodsService
    {
        Task<ResponseViewModel<List<PaymentMethodsDto>>> GetAllDtoAsync();
    }
}
