using System.Web;
using TikkoPayYonetici.Core;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly HttpClient _httpClient;

        public TelegramBotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendMessageAsync(string chatId, string text)
        {
            var url = $"https://api.telegram.org/bot{Constants.TelegramNorthItBotToken}/sendMessage?chat_id={HttpUtility.UrlEncode(chatId)}&text={HttpUtility.UrlEncode(text)}";

            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode;
        }

        public async Task SendLongMessageAsync(string chatId, string message, int chunkSize = 4000, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < message.Length; i += chunkSize)
            {
                var chunk = message.Substring(i, Math.Min(chunkSize, message.Length - i));
                await SendMessageAsync(chatId, chunk);

                await Task.Delay(300, cancellationToken);
            }
        }
    }
}
