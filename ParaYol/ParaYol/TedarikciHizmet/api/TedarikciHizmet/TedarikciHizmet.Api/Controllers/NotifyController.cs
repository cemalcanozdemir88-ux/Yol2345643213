using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TedarikciHizmet.Api.Hubs;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;

namespace TedarikciHizmet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _hubContext;

        public NotifyController(IHubContext<NotifyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send-yatirim-talebi")]
        public async Task<IActionResult> Send([FromBody] YatirimTalepleriManualListDto yatirimTalepleri)
        {
            if (yatirimTalepleri == null || yatirimTalepleri.TedarikciId.HasValue == false)
                return BadRequest("Message is required");

            var tedarikciId = yatirimTalepleri.TedarikciId.Value;

            await _hubContext.Clients.Group($"tedarikciId-{tedarikciId}").SendAsync("YeniYatirimTalebi", yatirimTalepleri);
            return Ok(new { sent = true });
        }
    }
}
