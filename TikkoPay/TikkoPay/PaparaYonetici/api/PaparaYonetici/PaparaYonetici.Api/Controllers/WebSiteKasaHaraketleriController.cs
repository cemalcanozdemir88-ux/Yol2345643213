using PaparaYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PaparaYonetici.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PaparaYonetici.Api.Controllers
{
    public class WebSiteKasaHaraketleriController : BaseController
    {
        private IWebSiteKasaHaraketleriService _webSiteKasaHaraketleriService;

        public WebSiteKasaHaraketleriController(IWebSiteKasaHaraketleriService kasaHaraketleriService)
        {
            _webSiteKasaHaraketleriService = kasaHaraketleriService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _webSiteKasaHaraketleriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid? webSiteId, int? durum, string? createdDateStart, string? createdDateEnd)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _webSiteKasaHaraketleriService.GetAll(webSiteId, durum, _createdDateStart, _createdDateEnd);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WebSiteKasaHaraketleriCreateDto item)
        {
            var result = await _webSiteKasaHaraketleriService.Create(item);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(WebSiteKasaHaraketleriUpdateDto item)
        {
            var result = await _webSiteKasaHaraketleriService.Update(item);
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
            var result = await _webSiteKasaHaraketleriService.Delete(id);
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
