using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Api.ActionFilters.PermissionControl;
using ParaYolYonetici.Api.Infrastructure;
using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
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
