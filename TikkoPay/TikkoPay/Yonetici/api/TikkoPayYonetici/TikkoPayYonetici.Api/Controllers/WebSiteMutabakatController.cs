using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs.WebSiteMutabakatlari;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class WebSiteMutabakatController : BaseController
    {
        private readonly IWebSiteMutabakatService _webSiteMutabakatService;

        public WebSiteMutabakatController(IWebSiteMutabakatService webSiteMutabakatService)
        {
            _webSiteMutabakatService = webSiteMutabakatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? webSiteId, string mutabakatTarihi)
        {
            var success = DateOnly.TryParseExact(
                mutabakatTarihi,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMutabakatTarihi
            );

            var items = await _webSiteMutabakatService.GetAllDtoAsync(webSiteId, parsedMutabakatTarihi);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(WebSiteMutabakatCalculateDto calculateDto)
        {
            var success = DateOnly.TryParseExact(
                calculateDto.MutabakatTarihi,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMutabakatTarihi
            );

            var result = await _webSiteMutabakatService.Calculate(calculateDto.WebSiteId, calculateDto.Devir, parsedMutabakatTarihi);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
