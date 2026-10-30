using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Core.DTOs.KullaniciBlackList;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class KullaniciBlackListController : BaseController
    {
        private readonly IKullaniciBlackListsService _kullaniciBlackListsService;

        public KullaniciBlackListController(IKullaniciBlackListsService kullaniciBlackListsService)
        {
            _kullaniciBlackListsService = kullaniciBlackListsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? websiteId, long? userId, int pageNumber, int recordsPerPage)
        {
            var items = await _kullaniciBlackListsService.GetAll(websiteId, userId, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullaniciBlackListsCreateDto item)
        {
            var result = await _kullaniciBlackListsService.Create(item);
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
            var result = await _kullaniciBlackListsService.Delete(id);
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
