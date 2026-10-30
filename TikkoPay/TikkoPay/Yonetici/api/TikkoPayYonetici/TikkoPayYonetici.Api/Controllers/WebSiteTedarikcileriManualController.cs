using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class WebSiteTedarikcileriManualController : BaseController
    {
        private readonly IWebSiteTedarikcileriManualService _webSiteTedarikcileriManualService;

        public WebSiteTedarikcileriManualController(IWebSiteTedarikcileriManualService webSiteTedarikcileriManualService)
        {
            _webSiteTedarikcileriManualService = webSiteTedarikcileriManualService;
        }

        [HttpGet("{webSiteId}")]
        public async Task<IActionResult> GetByWebSiteId(Guid webSiteId)
        {
            var items = await _webSiteTedarikcileriManualService.GetByWebSiteId(webSiteId);
            return Ok(items);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WebSiteTedarikcileriManualCreateDto item)
        {
            var result = await _webSiteTedarikcileriManualService.Create(item);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _webSiteTedarikcileriManualService.Delete(id);
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
