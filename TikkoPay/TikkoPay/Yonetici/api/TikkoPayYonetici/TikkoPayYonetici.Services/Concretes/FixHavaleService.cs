using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.FixHavale;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class FixHavaleService : IFixHavaleService
    {
        private readonly HttpClient _httpClient;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public FixHavaleService(HttpClient httpClient,
                                IWithdrawTransactionRepository withdrawTransactionRepository,
                                ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<FixHavaleWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new FixHavaleWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var apiKey = settings.FixHavaleApiKey;
            var apiSecret = settings.FixHavaleApiSecret;
            var path = settings.FixHavaleWithdrawUrl;

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

            var nameParts = SplitFullName(cekimTalepItem.AdSoyad);

            var payload = new
            {
                transactionId = cekimTalepItem.WebSiteTransactionId,
                sourceSiteId = "thy",
                sourceSiteName = "thy",
                userId = cekimTalepItem.UserId,
                username = cekimTalepItem.AdSoyad,
                firstName = nameParts.FirstName,
                middleName = nameParts.MiddleName,
                lastName = nameParts.LastName,
                amount = decimal.Truncate(cekimTalepItem.Tutar),
                bank_name = "fast",
                account_holder = cekimTalepItem.AdSoyad,
                iban = cekimTalepItem.HesapNo
            };

            var body = JsonConvert.SerializeObject(payload);

            var signature = Sign($"{timestamp}\n{nonce}\n{body}", apiSecret);

            using var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, path);
            request.Headers.Add("X-API-Key", apiKey);
            request.Headers.Add("X-API-Secret", apiSecret);
            request.Headers.Add("X-Timestamp", timestamp);
            request.Headers.Add("X-Nonce", nonce);
            request.Headers.Add("X-Signature", signature);

            if (body.Length > 0)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var postResponse = JsonConvert.DeserializeObject<FixHavaleWithdrawResponse>(json);

                if (response.IsSuccessStatusCode)
                {
                    postResponse!.IsSuccess = postResponse.Ok;

                    return postResponse;
                }
                else
                {
                    return new FixHavaleWithdrawResponse
                    {
                        IsSuccess = false,
                        Message = postResponse!.Transaction.Notes
                    };
                }
            }
            catch (Exception ex)
            {

                return new FixHavaleWithdrawResponse
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        private static string Sign(string value, string secret)
        => Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

        private class NameParts
        {
            public string FirstName { get; set; } = "";
            public string MiddleName { get; set; } = "";
            public string LastName { get; set; } = "";
        }

        private NameParts SplitFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new NameParts();

            var parts = fullName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return new NameParts
                {
                    FirstName = parts[0],
                    MiddleName = "",
                    LastName = ""
                };
            }

            if (parts.Length == 2)
            {
                return new NameParts
                {
                    FirstName = parts[0],
                    MiddleName = "",
                    LastName = parts[1]
                };
            }

            return new NameParts
            {
                FirstName = parts[0],
                MiddleName = string.Join(" ", parts.Skip(1).Take(parts.Length - 2)),
                LastName = parts[^1]
            };
        }
    }

    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = null,
            WriteIndented = false
        };
    }
}
