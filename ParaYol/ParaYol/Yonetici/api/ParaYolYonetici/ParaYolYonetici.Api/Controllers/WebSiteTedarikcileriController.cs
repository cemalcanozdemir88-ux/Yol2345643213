using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileri;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class WebSiteTedarikcileriController : BaseController
    {
        private readonly IWebSiteTedarikcileriService _webSiteTedarikcileriService;

        public WebSiteTedarikcileriController(IWebSiteTedarikcileriService webSiteTedarikcileriService)
        {
            _webSiteTedarikcileriService = webSiteTedarikcileriService;
        }

        [HttpGet("{webSiteId}")]
        public async Task<IActionResult> GetByWebSiteId(Guid webSiteId)
        {
            var items = await _webSiteTedarikcileriService.GetByWebSiteId(webSiteId);
            return Ok(items);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WebSiteTedarikcileriCreateDto item)
        {
            var result = await _webSiteTedarikcileriService.Create(item);
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
            var result = await _webSiteTedarikcileriService.Delete(id);
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
