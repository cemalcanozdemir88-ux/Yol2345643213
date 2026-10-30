using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class WebSitePaymentMethodsController : BaseController
    {
        private readonly IWebSitePaymentMethodService _webSitePaymentMethodService;

        public WebSitePaymentMethodsController(IWebSitePaymentMethodService webSitePaymentMethodService)
        {
            _webSitePaymentMethodService = webSitePaymentMethodService;
        }

        [HttpGet("get-by-websiteid")]
        public async Task<IActionResult> GetByWebSiteId([FromQuery] Guid webSiteId)
        {
            var result = await _webSitePaymentMethodService.GetByWebSiteIdDtoAsync(webSiteId);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
