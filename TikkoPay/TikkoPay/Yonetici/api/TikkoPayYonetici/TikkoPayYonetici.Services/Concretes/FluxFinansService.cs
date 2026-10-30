using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.FluxFinans;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class FluxFinansService : IFluxFinansService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public FluxFinansService(HttpClient httpClient,
                                 ISettingsV2Repository settingsV2Repository, 
                                 IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<FluxFinansWithdrawResponse> SendWithdrawToFluxFinanceAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new FluxFinansWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();
            var token = await GetTokenAsync(settings);

            var payload = new
            {
                token = token,
                transaction = new
                {
                    transaction_id = cekimTalepItem.WebSiteTransactionId,
                    user_id = cekimTalepItem.UserId,
                    username = cekimTalepItem.UserName,
                    fullname = cekimTalepItem.AdSoyad,
                    receiver_bank_name = "FAST",
                    receiver_account_holder_name = cekimTalepItem.HesapSahibi,
                    receiver_iban = cekimTalepItem.HesapNo,
                    amount = cekimTalepItem.Tutar,
                    hash = GenerateHash(cekimTalepItem.WebSiteTransactionId, cekimTalepItem.UserId, cekimTalepItem.Tutar.ToString("F2", CultureInfo.InvariantCulture), settings.FluxFinansSecretKey)
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(settings.FluxFinansWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<FluxFinansWithdrawResponse>(json);

            string rawContent = await content.ReadAsStringAsync();
            await LogPayloadAsync(settings.FluxFinansWithdrawUrl, rawContent, json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Status == "true";

                return postResponse;
            }
            else
            {
                return new FluxFinansWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "Error when sending witdrah request",
                };
            }
        }

        private async Task LogPayloadAsync(string callbackUrl, string payload, string response)
        {
            var logDirectory = Path.Combine("logs", "request", DateTime.Now.ToString("yyyy-MM-dd"), "flux-finans");
            var logFilePath = Path.Combine(logDirectory, $"log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (var writer = new StreamWriter(logFilePath, append: false, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("/******** Request Url ********/");
                sb.AppendLine(callbackUrl);
                sb.AppendLine("/******** Request ********/");
                sb.AppendLine(payload);
                sb.AppendLine(" ");
                sb.AppendLine("/******** Response ********/");
                sb.AppendLine(response);

                await writer.WriteAsync(sb.ToString());
            }
        }


        private string GenerateHash(string transactionId, string userId, string amount, string secretkey)
        {
            var normalizedAmount = decimal
                .Parse(amount.Trim().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
                .ToString("F2", CultureInfo.InvariantCulture);

            var payload = $"{transactionId}{userId}{amount}{secretkey}";

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private async Task<string> GetTokenAsync(Settings settings)
        {
            var payload = new
            {
                sid = settings.FluxFinansSid,
                apikey = settings.FluxFinansApiKey
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(settings.FluxFinansAuthUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<FluxFinansTokenResponse>(json);

                return tokenResponse.Data.Token;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Token alma işlemi başarısız: {error}");
            }
        }
    }
}
