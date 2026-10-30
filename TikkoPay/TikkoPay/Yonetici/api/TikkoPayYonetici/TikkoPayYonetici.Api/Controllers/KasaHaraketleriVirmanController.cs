using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Api.ActionFilters.PermissionControl;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core.DTOs.KasaHaraketleri;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
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
