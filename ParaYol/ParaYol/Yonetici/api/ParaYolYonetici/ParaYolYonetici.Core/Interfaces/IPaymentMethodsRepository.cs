using ParaYolYonetici.Core.DTOs.PaymentMethods;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IPaymentMethodsRepository
    {
        Task<IEnumerable<PaymentMethodsDto>> GetAllDtoAsync();
    }
}
