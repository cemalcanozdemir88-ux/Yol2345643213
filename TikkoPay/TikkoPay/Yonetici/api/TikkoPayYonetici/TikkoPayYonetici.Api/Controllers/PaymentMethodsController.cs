using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class PaymentMethodsController : BaseController
    {
        private readonly IPaymentMethodsService _paymentMethodsService;

        public PaymentMethodsController(IPaymentMethodsService paymentMethodsService)
        {
            _paymentMethodsService = paymentMethodsService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentMethodsService.GetAllDtoAsync();
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
