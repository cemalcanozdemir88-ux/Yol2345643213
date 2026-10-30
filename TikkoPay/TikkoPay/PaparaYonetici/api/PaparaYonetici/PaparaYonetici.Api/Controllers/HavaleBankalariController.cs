using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaparaYonetici.Core.DTOs.HavaleBankalari;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Services.Interfaces;

namespace PaparaYonetici.Api.Controllers
{
    public class HavaleBankalariController : BaseController
    {
        private readonly IHavaleBankalariService _havaleBankalariService;

        public HavaleBankalariController(IHavaleBankalariService havaleBankalariService)
        {
            _havaleBankalariService = havaleBankalariService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _havaleBankalariService.GetById(id);
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
        public async Task<IActionResult> GetAll([FromQuery] Guid? tedarikciId, Guid? bankaId, string? searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage)
        {
            searchText ??= string.Empty;

            var bankalar = await _havaleBankalariService.GetAll(tedarikciId, bankaId, searchText, havaleBankalariDurumFilter, pageNumber, recordsPerPage);
            return Ok(bankalar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HavaleBankalariCreateDto banka)
        {
            var result = await _havaleBankalariService.Create(banka);
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
        public async Task<IActionResult> Update(HavaleBankalariUpdateDto banka)
        {

            var result = await _havaleBankalariService.Update(banka);
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
            var result = await _havaleBankalariService.Delete(id);
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
