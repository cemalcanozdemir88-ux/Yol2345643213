namespace TedarikciHizmet.Services.Interfaces
{
    public interface ITelegramBotService
    {
        Task<bool> SendMessageAsync(string chatId, string text);
    }
}
