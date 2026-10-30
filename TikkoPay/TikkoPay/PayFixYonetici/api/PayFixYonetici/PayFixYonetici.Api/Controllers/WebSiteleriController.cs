using Microsoft.AspNetCore.Mvc;
using PayFixYonetici.Core.DTOs.WebSiteleri;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Api.Controllers
{
    public class WebSiteleriController : BaseController
    {
        public readonly IWebSiteleriService _webSiteleriService;

        public WebSiteleriController(IWebSiteleriService webSiteleriService)
        {
            _webSiteleriService = webSiteleriService;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var item = _webSiteleriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            Guid? webSiteId = null;
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            var items = await _webSiteleriService.GetAllAsync(webSiteId);
            return Ok(items);
        }

        [HttpGet("get-select-list")]
        [ResponseCache(Duration = 900, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetSelectList()
        {
            Guid? webSiteId = null;
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            var items = await _webSiteleriService.GetSelectListAsync(webSiteId);
            return Ok(items);
        }

        [HttpGet("get-select-list-by-affiliate")]
        public async Task<IActionResult> GetSelectList([FromQuery] Guid affiliateId)
        {
            var items = await _webSiteleriService.GetSelectListByAffiliateAsync(affiliateId);
            return Ok(items);
        }

        [HttpGet("get-bakiyelist")]
        public async Task<IActionResult> GetBakiyeList([FromQuery] Guid? webSiteId)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            var tedarikciler = await _webSiteleriService.GetBakiyeList(webSiteId);
            return Ok(tedarikciler);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WebSiteleriCreateDto webSiteleri)
        {
            var result = await _webSiteleriService.Create(webSiteleri);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(WebSiteleriUpdateDto webSiteleri)
        {
            var result = await _webSiteleriService.Update(webSiteleri);
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
            var result = await _webSiteleriService.Delete(id);
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
