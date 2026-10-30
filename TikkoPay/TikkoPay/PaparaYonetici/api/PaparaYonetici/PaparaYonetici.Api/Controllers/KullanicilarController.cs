using PaparaYonetici.Core.DTOs.Kullanicilar;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PaparaYonetici.Api.Controllers
{
    public class KullanicilarController : BaseController
    {
        private readonly IKullanicilarService _kullanicilarService;

        public KullanicilarController(IKullanicilarService kullanicilarService)
        {
            _kullanicilarService = kullanicilarService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _kullanicilarService.GetById(id);
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
        public async Task<IActionResult> GetAll(KullaniciTip? kullaniciTipi)
        {
            var kullanicilar = await _kullanicilarService.GetAll(kullaniciTipi);
            return Ok(kullanicilar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullanicilarCreateDto kullanicilarCreate)
        {
            var result = await _kullanicilarService.Create(kullanicilarCreate);
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
        public async Task<IActionResult> Update(KullanicilarUpdateDto kullanicilarUpdate)
        {
            var result = await _kullanicilarService.Update(kullanicilarUpdate);
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
            var result = await _kullanicilarService.Delete(id);
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
