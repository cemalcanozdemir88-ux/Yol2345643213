using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TikkoPayYonetici.Api.Hubs;

namespace TikkoPayYonetici.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _hubContext;

        public NotifyController(IHubContext<NotifyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("update-havale-banka-list")]
        public async Task<IActionResult> Send([FromBody] string websiteId)
        {
            await _hubContext.Clients.Group($"websiteId-{websiteId}").SendAsync("UpdateHavaleBankaList");
            return Ok(new { sent = true });
        }
    }
}
