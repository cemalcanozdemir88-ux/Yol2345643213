using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Core.DTOs.HavaleBankalari;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
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
            List<Guid> fastBankIds = new List<Guid>
            {
                 Guid.Parse("7b45e338-626e-47a0-a705-acc93b24eeab"),
                 Guid.Parse("439ba9a5-4cee-4744-9032-b6f03ee34000")
            };

            if (!fastBankIds.Contains(banka.BankaId))
            {
                return BadRequest("sadece fast banka ekleneblir");
            }

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
            List<Guid> fastBankIds = new List<Guid>
            {
                 Guid.Parse("7b45e338-626e-47a0-a705-acc93b24eeab"),
                 Guid.Parse("439ba9a5-4cee-4744-9032-b6f03ee34000")
            };

            if (!fastBankIds.Contains(banka.BankaId))
            {
                return BadRequest("sadece fast banka ekleneblir");
            }

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
