using PaparaTedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using PaparaTedarikciHizmet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PaparaTedarikciHizmet.Api.Controllers
{
    public class KullaniciYetkileriController : BaseController
    {
        private readonly IKullaniciYetkileriService _kullaniciYetkileriService;

        public KullaniciYetkileriController(IKullaniciYetkileriService kullaniciYetkileriService)
        {
            _kullaniciYetkileriService = kullaniciYetkileriService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _kullaniciYetkileriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet("get-by-kullaniciId")]
        public async Task<IActionResult> GetByKullaniciId([FromQuery] Guid kullaniciId)
        {
            var items = await _kullaniciYetkileriService.GetByKullaniciId(kullaniciId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullaniciYetkileriCreateDto createDto)
        {
            var result = await _kullaniciYetkileriService.Create(createDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(KullaniciYetkileriDeleteDto deleteDto)
        {
            var result = await _kullaniciYetkileriService.Delete(deleteDto);
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
