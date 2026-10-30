using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Api.ActionFilters.PermissionControl;
using ParaYolYonetici.Api.Infrastructure;
using ParaYolYonetici.Core.DTOs.KasaHaraketleri;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KasaHaraketleriVirmanController : ControllerBase
    {
        private readonly IKasaHaraketleriService _kasaHaraketleriService;

        public KasaHaraketleriVirmanController(IKasaHaraketleriService kasaHaraketleriService)
        {
            _kasaHaraketleriService = kasaHaraketleriService;
        }

        [PermissionAuthorize(Yetki.TedarikciKasaHaraketleriVirmanEkleyebilir)]
        [HttpPost]
        public async Task<IActionResult> Create(KasaHaraketleriVirmanCreateDto item)
        {
            var result = await _kasaHaraketleriService.CreateVirman(item);
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
