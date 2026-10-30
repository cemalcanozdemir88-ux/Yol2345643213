using TikkoPayYonetici.Core.DTOs.PaymentMethods;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IPaymentMethodsRepository
    {
        Task<IEnumerable<PaymentMethodsDto>> GetAllDtoAsync();
    }
}
