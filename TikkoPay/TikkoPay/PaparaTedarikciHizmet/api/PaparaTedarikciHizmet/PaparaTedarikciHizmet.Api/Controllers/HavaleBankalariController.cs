using Microsoft.AspNetCore.Mvc;
using PaparaTedarikciHizmet.Core.DTOs.HavaleBankalari;
using PaparaTedarikciHizmet.Core.Enums;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Api.Controllers
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
        public async Task<IActionResult> GetAll([FromQuery] Guid? bankaId, string? searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage)
        {
            var bankalar = await _havaleBankalariService.GetAll(bankaId, searchText, havaleBankalariDurumFilter, pageNumber, recordsPerPage);
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
    }
}
