using Newtonsoft.Json;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Infrastructure.Services.Interfaces;
using NorthHavaleTransactions.Models.FluxFinans;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace NorthHavaleTransactions.Infrastructure.Services.Concretes
{
    public class FluxFinansService : IFluxFinansService
    {
        private readonly HttpClient _httpClient;
        private ISettingsV2Repository _settingsV2Repository;

        public FluxFinansService(HttpClient httpClient, ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<FluxFinansBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();
            var token = await GetTokenAsync(settings);

            var payload = new
            {
                token = token,
                transaction = new
                {
                    transaction_id = depositTransaction.TransactionId,
                    user_id = depositTransaction.UserId,
                    username = depositTransaction.UserName,
                    fullname = depositTransaction.FullName,
                    amount = depositTransaction.Amount,
                    hash = GenerateHash(depositTransaction.TransactionId, depositTransaction.UserId, amount.ToString("F2", CultureInfo.InvariantCulture), settings.FluxFinansSecretKey)
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(settings.FluxFinansDepositUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<FluxFinansBankAccountResponse>(json);

                return bankAccountResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Banka hesap bilgileri alma işlemi başarısız: TransactionId: {depositTransaction.TransactionId} - {error}");
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
