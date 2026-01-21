namespace OmniChannel.API.Services
{
    public interface ILogService
    {
        Task LogAsync(string message, string level = "INFO");
    }
}
