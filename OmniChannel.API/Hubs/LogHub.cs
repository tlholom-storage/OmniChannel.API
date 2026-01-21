using Microsoft.AspNetCore.SignalR;

namespace OmniChannel.API.Hubs
{
    public class LogHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveSystemLog",
                    $"[INFO] {DateTime.Now:HH:mm:ss} - Connected to LogHub.");

            await base.OnConnectedAsync();
        }
    }
}
