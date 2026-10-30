using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Api.ActionFilters.PermissionControl;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSiteKasaHaraketleriVirmanController : ControllerBase
    {
        private IWebSiteKasaHaraketleriService _webSiteKasaHaraketleriService;

        public WebSiteKasaHaraketleriVirmanController(IWebSiteKasaHaraketleriService webSiteKasaHaraketleriService)
        {
            _webSiteKasaHaraketleriService = webSiteKasaHaraketleriService;
        }

        [PermissionAuthorize(Yetki.SiteKasaHaraketleriVirmanEkleyebilir)]
        [HttpPost]
        public async Task<IActionResult> Create(WebSiteKasaHaraketleriVirmanCreateDto item)
        {
            var result = await _webSiteKasaHaraketleriService.CreateVirman(item);
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
