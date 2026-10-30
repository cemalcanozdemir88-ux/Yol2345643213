using Microsoft.AspNetCore.Mvc;
using PayFixYonetici.Core.DTOs;
using PayFixYonetici.Core.DTOs.KullaniciYetkileri;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Api.Controllers
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
            if (IsWebSiteUser())
            {
                RemoveAffiliateYonetimiFromYetkiler(items);
                RemoveSiteTanimlariFromYetkiler(items);
            }

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

        private static void RemoveAffiliateYonetimiFromYetkiler(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("11D4E951-44DA-45AF-B5EB-511C7590917E"), // Affiliate Yonetimi
                Guid.Parse("CDDEE3F8-F056-40AE-BD12-C69D1D23AAA6"), // Affiliate Tanimlari
                Guid.Parse("164DF90B-E47D-47D6-B4A9-5FEE39988B26"), // Affiliate Tanimlari Ekleyebilir
                Guid.Parse("34A14D83-B1E1-46E0-B93B-751AF5834C33"), // Affiliate Tanimlari Silebilir
                Guid.Parse("484C2B95-9357-400E-B961-D2A6612AF955"), // Affiliate Tanimlari Duzenleyebilir
                Guid.Parse("FEBBAFB1-3969-4CC4-95B2-F801459478F2"), // Affiliate Tanimlari Gorebilir

                Guid.Parse("AB746774-5894-4DFE-803E-74A5645BDEA4"), // Affiliate Bakiye
                Guid.Parse("568A803A-94B6-4A80-8BCE-782E7DE096EB"), // Affiliate Bakiye Görebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveSiteTanimlariFromYetkiler(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("1A86378B-CEF6-4262-95AE-187619909F86"), // Site Tanımları
                Guid.Parse("A25A803D-3AEE-4B92-8BEF-19481A87EF0B"), // Web Sitesi Düzenleyebilir
                Guid.Parse("FE9C43C3-41D3-4D7F-AAC2-437FB0A6A40D"), // Site Tanımları Görebilir
                Guid.Parse("D0E2AD3C-340C-49BD-A108-DC2ABF1BE5CA"), // Web Sitesi Silebilir
                Guid.Parse("6DB0A7B5-42CA-4F66-8780-F6CA798565C1"), // Web Sitesi Ekleyebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }
    }
}
