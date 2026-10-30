using Microsoft.AspNetCore.Mvc;
using PayFixYonetici.Core.DTOs.Kullanicilar;
using PayFixYonetici.Core.Enums;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Api.Controllers
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
            Guid? webSiteId = null;
            if (IsWebSiteUser())
            {
                webSiteId = GetWebSiteId();
            }

            Guid? affiliateId = null;
            if (IsAffiliateUser())
            {
                affiliateId = GetAffiliateId();
            }

            var kullanicilar = await _kullanicilarService.GetAll(kullaniciTipi, webSiteId, affiliateId);
            return Ok(kullanicilar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullanicilarCreateDto kullanicilarCreate)
        {
            if (IsWebSiteUser())
            {
                kullanicilarCreate.WebSiteId = GetWebSiteId();
                kullanicilarCreate.KullaniciTipi = KullaniciTip.WebSiteKullanicisi;
            }

            if (IsAffiliateUser())
            {
                kullanicilarCreate.AffiliateId = GetAffiliateId();
                kullanicilarCreate.KullaniciTipi = KullaniciTip.AffiliateKullanicisi;
            }

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
