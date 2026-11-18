using System.Globalization;
using System.Text.Json;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Concretes
{
    public class BlueParaApiClient : IBlueParaApiClient
    {
        private readonly HttpClient _httpClient;

        public BlueParaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BlueParaTransactionInfo?> GetDepositAsync(string transactionId, Settings settings)
        {
            using var doc = await GetAsync($"merchant-api/gateway/transactions/{Uri.EscapeDataString(transactionId)}", settings);
            if (doc is null)
                return null;

            var root = doc.RootElement;
            // Bazı cevaplar { success, data: {...} } şeklinde sarılı olabilir
            if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                root = data;

            return new BlueParaTransactionInfo
            {
                Status = GetString(root, "status") ?? string.Empty,
                Amount = GetDecimal(root, "receivedAmount") ?? GetDecimal(root, "amount") ?? 0,
                CallbackUrl = GetString(root, "callback_url") ?? GetString(root, "callbackUrl"),
                MerchantTransactionId = GetString(root, "merchant_transaction_id"),
            };
        }

        public async Task<BlueParaTransactionInfo?> GetWithdrawalAsync(string reference, Settings settings)
        {
            using var doc = await GetAsync($"merchant-api/withdrawals/{Uri.EscapeDataString(reference)}", settings);
            if (doc is null)
                return null;

            var root = doc.RootElement;
            if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                root = data;

            return new BlueParaTransactionInfo
            {
                Status = GetString(root, "status") ?? string.Empty,
                Amount = GetDecimal(root, "amount") ?? 0,
                MerchantTransactionId = GetString(root, "merchant_transaction_id"),
            };
        }

        private async Task<JsonDocument?> GetAsync(string path, Settings settings)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{settings.BlueParaBaseUrl.TrimEnd('/')}/{path}");
            request.Headers.Add("X-API-KEY", settings.BlueParaApiKey.Trim());
            // Canlı hesap Provider Key ile çalışıyor; istek oluştururken olduğu gibi sorguda da gönderilir
            if (!string.IsNullOrWhiteSpace(settings.BlueParaProviderKey))
                request.Headers.Add("X-PROVIDER-KEY", settings.BlueParaProviderKey.Trim());

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        private static string? GetString(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        private static decimal? GetDecimal(JsonElement e, string name)
        {
            if (!e.TryGetProperty(name, out var v))
                return null;

            if (v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out var d))
                return d;

            if (v.ValueKind == JsonValueKind.String &&
                decimal.TryParse(v.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var s))
                return s;

            return null;
        }
    }
}
