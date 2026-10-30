namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface ITelegramBotService
    {
        Task<bool> SendMessageAsync(string chatId, string text);
        Task SendLongMessageAsync(string chatId, string message, int chunkSize = 4000, CancellationToken cancellationToken = default);
    }
}
