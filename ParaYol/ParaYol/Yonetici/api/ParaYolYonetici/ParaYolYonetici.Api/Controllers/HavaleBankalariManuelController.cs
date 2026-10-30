using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class HavaleBankalariManuelController : BaseController
    {
        private readonly IHavaleBankalariService _havaleBankalariService;

        public HavaleBankalariManuelController(IHavaleBankalariService havaleBankalariService)
        {
            _havaleBankalariService = havaleBankalariService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var webSiteId = GetWebSiteId();
            var havaleBankalari = await _havaleBankalariService.GetAllByWebSiteTedarikcileri(webSiteId);

            return Ok(havaleBankalari);
        }
    }
}
