using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.AsPay;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class AsPayServices : IAsPayServices
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AsPayServices(HttpClient httpClient,
                             ISettingsRepository settingsRepository,
                             IWithdrawTransactionRepository withdrawTransactionRepository,
                             IUnitOfWork unitOfWork
                             )
        {
            _httpClient = httpClient;
            _settingsRepository = settingsRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AsPayWithdrawResponse> SendWithdrawToAsPayAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new AsPayWithdrawResponse
                {
                    IsPostRequestSuccess = false,
                    Status = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsRepository.GetFirst();
            var token = await GetTokenAsync(settings);

            var nameSurname = ParseFullName(cekimTalepItem.AdSoyad);

            var payload = new
            {
                type = "Bank Transfer",
                account_name = cekimTalepItem.HesapSahibi,
                account_number = cekimTalepItem.HesapNo,
                amount = cekimTalepItem.Tutar,
                user = new
                {
                    id = cekimTalepItem.UserId,
                    name = nameSurname.Name,
                    surname = nameSurname.Surname,
                    username = cekimTalepItem.UserName,
                },
                transaction_id = withDrawTransactionDbItem.Id,
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(settings.AsPayWithdrawUrl, content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await GetTokenAsync(settings, true);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                response = await _httpClient.PostAsync(settings.AsPayWithdrawUrl, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return new AsPayWithdrawResponse
                {
                    IsPostRequestSuccess = true,
                    Status = true,
                    Message = string.Empty
                };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new AsPayWithdrawResponse
                {
                    IsPostRequestSuccess = false,
                    Status = false,
                    Message = error
                };
            }
        }

        private async Task<string> GetTokenAsync(Settings settings, bool getFromApi = false)
        {
            if (!getFromApi && !string.IsNullOrEmpty(settings.AsPayToken) && settings.AsPayTokenExpiresAt > DateTime.Now)
            {
                return settings.AsPayToken;
            }

            var payload = new
            {
                app_key = settings.AsPayAppKey,
                app_secret = settings.AsPayAppSecret
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(settings.AsPayAuthUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<AsPayTokenResponse>(json);
                var token = tokenResponse.Token;
                var tokenExpiry = DateTime.ParseExact(tokenResponse.Expires, "dd.MM.yyyy HH:mm:ss", new CultureInfo("tr-TR"));

                settings.AsPayToken = token;    
                settings.AsPayTokenExpiresAt = tokenExpiry;

                return token;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Token alma işlemi başarısız: {error}");
            }
        }

        private PersonName ParseFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new PersonName();

            string cleanedName = Regex.Replace(fullName, @"\([^)]*\)", "").Trim();

            var tokens = cleanedName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                return new PersonName();

            if (tokens.Length == 1)
                return new PersonName { Name = tokens[0], Surname = string.Empty };

            string firstName = string.Join(" ", tokens.Take(tokens.Length - 1));
            string surname = tokens.Last();

            return new PersonName { Name = firstName, Surname = surname };
        }

        private class PersonName
        {
            public string Name { get; set; }
            public string Surname { get; set; }
        }
    }
}
