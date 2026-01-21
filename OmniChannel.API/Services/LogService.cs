using Microsoft.AspNetCore.SignalR;
using OmniChannel.API.Hubs;

namespace OmniChannel.API.Services
{
    public class LogService : ILogService
    {
        private readonly IHubContext<LogHub> _hubContext;

        public LogService(IHubContext<LogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task LogAsync(string message, string level = "INFO")
        {
            var formattedMessage =
                $"[{level}] {DateTime.Now:HH:mm:ss} - {message}";

            await _hubContext.Clients.All.SendAsync(
                "ReceiveSystemLog",
                formattedMessage
            );
        }
    }
}
