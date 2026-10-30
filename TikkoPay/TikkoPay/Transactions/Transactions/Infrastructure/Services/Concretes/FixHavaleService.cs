using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.FixHavale;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class FixHavaleService : IFixHavaleService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public FixHavaleService(HttpClient httpClient, 
                                ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        

        public async Task<FixApiDepositResponseDto> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var nameParts = SplitFullName(depositTransaction.FullName);

            var payload = new
            {
                transactionId = depositTransaction.TransactionId,
                sourceSiteId = "thy",
                sourceSiteName = "thy",
                userId = depositTransaction.UserId,
                username = depositTransaction.UserName,
                firstName = nameParts.FirstName,
                middleName = nameParts.MiddleName,
                lastName = nameParts.LastName,
                amount = amount,
                currency = "TRY",
            };

            var body = payload is null ? string.Empty : JsonSerializer.Serialize(payload, JsonOptions.Default);

            var response = await RequestAsync<FixApiDepositResponseDto>(body);
            return response;
        }

        private async Task<T> RequestAsync<T>(string body)
        {
            var responseResult = await SendAsync(body);
            
            using var json = responseResult.Body;

            var result = json.RootElement.Deserialize<T>(JsonOptions.Default)
                ?? throw new JsonException("API response could not be converted to the requested DTO.");
            
            if (result is IFixApiResponse apiResponse)
                apiResponse.IsSuccessStatusCode = responseResult.IsSuccessStatusCode;
            
            return result;
        }

        private async Task<(JsonDocument Body, bool IsSuccessStatusCode)> SendAsync(string body)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var apiKey = settings.FixHavaleApiKey; 
            var apiSecret = settings.FixHavaleApiSecret; 
            var path = settings.FixHavaleDepositUrl;

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
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
            var responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument json = null;
            if (!string.IsNullOrWhiteSpace(responseBody))
                json = JsonDocument.Parse(responseBody);

            return (json ?? JsonDocument.Parse("{}"), response.IsSuccessStatusCode);
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
