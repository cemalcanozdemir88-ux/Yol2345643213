using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.BluePara;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class BlueParaService : IBlueParaService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public BlueParaService(HttpClient httpClient, ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<BlueParaGenerateLinkResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            // callbackUrl'e bizim transaction numaramız eklenir; callback gelince BluePara'dan işlem sorgulanıp
            // bu adres karşılaştırılır, böylece bildirim doğru kayda bağlanır.
            var callbackUrl = $"{settings.BlueParaCallbackUrl}?ref={Uri.EscapeDataString(depositTransaction.TransactionId)}";

            var payload = new Dictionary<string, object>
            {
                ["amount"] = amount,
                ["currency"] = "TRY",
                ["senderFullName"] = depositTransaction.FullName ?? string.Empty,
                // Aynı işlem yöneticiden yeniden gönderilebildiği için her istekte benzersiz olmalı
                ["merchant_transaction_id"] = $"{depositTransaction.TransactionId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                ["callbackUrl"] = callbackUrl,
            };

            if (!string.IsNullOrWhiteSpace(depositTransaction.UserId))
                payload["merchant_user_id"] = depositTransaction.UserId;

            // İki giriş yöntemi var: Provider Key verildiyse eski yöntem (header'da X-PROVIDER-KEY, hash yok),
            // yoksa yeni yöntem (Merchant Key ile hesaplanan hash gövdede).
            var useProviderKey = !string.IsNullOrWhiteSpace(settings.BlueParaProviderKey);
            if (!useProviderKey)
                payload["hash"] = BlueParaSigner.Sign(BlueParaSigner.Stringify(payload), settings.BlueParaMerchantKey);

            var body = BlueParaSigner.Stringify(payload);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.BlueParaBaseUrl.TrimEnd('/')}/merchant-api/gateway/generate-link");
            request.Headers.Add("X-API-KEY", settings.BlueParaApiKey.Trim());
            if (useProviderKey)
                request.Headers.Add("X-PROVIDER-KEY", settings.BlueParaProviderKey.Trim());
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            BlueParaGenerateLinkResponse result = null;
            try
            {
                // Cevap düz ya da { success, data: {...} } şeklinde sarılı gelebilir
                var root = JObject.Parse(json);
                var data = root["data"] as JObject;
                result = (data ?? root).ToObject<BlueParaGenerateLinkResponse>();
                result.success ??= root["success"]?.Type == JTokenType.Boolean ? root.Value<bool>("success") : null;
                result.message ??= root.Value<string>("message");
            }
            catch
            {
            }

            if (response.IsSuccessStatusCode && result != null)
            {
                if (string.IsNullOrWhiteSpace(result.bankAccount?.iban))
                {
                    // Hata ayıklamak için BluePara'nın ham cevabı log'a düşer (müşteriye gösterilmez)
                    throw new Exception($"BluePara hesap bilgisi döndürmedi. TransactionId: {depositTransaction.TransactionId} - BluePara cevabı: {Truncate(json, 2000)}");
                }

                return result;
            }

            var error = result?.message;
            if (result?.errors?.Count > 0)
                error = $"{error} ({string.Join(", ", result.errors.Select(e => $"{e.field}: {e.message}"))})";

            throw new Exception($"BluePara banka hesap bilgisi alınamadı: TransactionId: {depositTransaction.TransactionId} - HTTP {(int)response.StatusCode} - {(string.IsNullOrWhiteSpace(error) ? Truncate(json, 2000) : error)}");
        }

        private static string Truncate(string value, int max)
            => string.IsNullOrEmpty(value) || value.Length <= max ? value : value.Substring(0, max) + "...";
    }
}
