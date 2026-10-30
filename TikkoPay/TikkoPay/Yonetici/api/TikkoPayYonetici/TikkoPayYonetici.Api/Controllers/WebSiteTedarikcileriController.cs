using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileri;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
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

        [HttpGet("by-tedarikci/{tedarikciId}")]
        public async Task<IActionResult> GetByTedarikciId(Guid tedarikciId)
        {
            var items = await _webSiteTedarikcileriService.GetByTedarikciId(tedarikciId);
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
