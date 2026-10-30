using Microsoft.AspNetCore.SignalR;

namespace TikkoPayYonetici.Api.Hubs
{
    public class NotifyHub : Hub
    {
        private readonly ILogger<NotifyHub> _logger;

        public NotifyHub(ILogger<NotifyHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var websiteId = Context.User?.FindFirst("websiteId")?.Value;

            if (!string.IsNullOrEmpty(websiteId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"websiteId-{websiteId}");
            }

            //_logger.LogInformation("NotifyHub OnConnected: {connectionId} user: {user}", Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception? exception)
        {
            //_logger.LogInformation("NotifyHub OnDisconnected: {connectionId} reason: {reason}", Context.ConnectionId, exception?.Message);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
