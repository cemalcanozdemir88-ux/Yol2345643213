using Microsoft.AspNetCore.SignalR;

namespace TedarikciHizmet.Api.Hubs
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
            var tedarikciId = Context.User?.FindFirst("tedarikciId")?.Value;

            if (!string.IsNullOrEmpty(tedarikciId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"tedarikciId-{tedarikciId}");
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
