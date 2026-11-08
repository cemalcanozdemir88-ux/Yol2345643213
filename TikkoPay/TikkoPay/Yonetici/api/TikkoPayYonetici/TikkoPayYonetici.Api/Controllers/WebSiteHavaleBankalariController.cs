using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class WebSiteHavaleBankalariController : BaseController
    {
        private readonly IWebSiteHavaleBankalariService _webSiteHavaleBankalariService;

        public WebSiteHavaleBankalariController(IWebSiteHavaleBankalariService webSiteHavaleBankalariService)
        {
            _webSiteHavaleBankalariService = webSiteHavaleBankalariService;
        }

        [HttpGet("{webSiteId}")]
        public async Task<IActionResult> GetByWebSiteId(Guid webSiteId)
        {
            return Ok(await _webSiteHavaleBankalariService.GetByWebSiteId(webSiteId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WebSiteHavaleBankalariCreateDto item)
        {
            var result = await _webSiteHavaleBankalariService.Create(item);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _webSiteHavaleBankalariService.Delete(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
