using System.Web;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
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
            var url = $"https://api.telegram.org/bot{TelegramBotConstants.ParaYolItBotToken}/sendMessage?chat_id={HttpUtility.UrlEncode(chatId)}&text={HttpUtility.UrlEncode(text)}";

            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode;
        }
    }
}
