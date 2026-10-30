using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Core.DTOs.KullaniciIpAdresleri;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class KullaniciIpAdresleriController : BaseController
    {
        private readonly IKullaniciIpAdresleriService _kullaniciIpAdresleriService;

        public KullaniciIpAdresleriController(IKullaniciIpAdresleriService kullaniciIpAdresleriService)
        {
            _kullaniciIpAdresleriService = kullaniciIpAdresleriService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _kullaniciIpAdresleriService.GetById(id);
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
        public async Task<IActionResult> GetAll([FromQuery] Guid? tedarikciId, string? searchText)
        {
            searchText ??= string.Empty;

            var items = await _kullaniciIpAdresleriService.GetAll(tedarikciId, searchText);
            return Ok(items);
        }

        [HttpGet("get-by-kullaniciId")]
        public async Task<IActionResult> GetByKullaniciId([FromQuery] Guid kullaniciId)
        {
            var items = await _kullaniciIpAdresleriService.GetByKullaniciId(kullaniciId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullaniciIpAdresleriCreateDto kullaniciIpAdresleri)
        {
            var result = await _kullaniciIpAdresleriService.Create(kullaniciIpAdresleri);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update-ip-list")]
        public async Task<IActionResult> UpdateIpList(UpdateIpListDto updateIpListDto)
        {
            var result = await _kullaniciIpAdresleriService.UpdateIpList(updateIpListDto);
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
        public async Task<IActionResult> Update(KullaniciIpAdresleriUpdateDto kullaniciIpAdresleri)
        {

            var result = await _kullaniciIpAdresleriService.Update(kullaniciIpAdresleri);
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
            var result = await _kullaniciIpAdresleriService.Delete(id);
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
