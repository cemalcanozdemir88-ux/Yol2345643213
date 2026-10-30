using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.PaymentMethods;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IPaymentMethodsService
    {
        Task<ResponseViewModel<List<PaymentMethodsDto>>> GetAllDtoAsync();
    }
}
