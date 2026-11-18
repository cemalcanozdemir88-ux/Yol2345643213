using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.BluePara;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Helpers;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
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

        public async Task<BlueParaWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            // İç içe nesneler imzada gönderildiği sırayla kullanılır, bu yüzden sıralı sözlük.
            var accountInfo = new SortedDictionary<string, object>(StringComparer.Ordinal)
            {
                ["accountHolderName"] = cekimTalepItem.AdSoyad ?? string.Empty,
                ["accountNumber"] = cekimTalepItem.HesapNo ?? string.Empty,
                ["iban"] = cekimTalepItem.HesapNo ?? string.Empty,
            };

            var payload = new Dictionary<string, object>
            {
                ["amount"] = cekimTalepItem.Tutar,
                ["currency"] = "TRY",
                ["senderFullName"] = cekimTalepItem.AdSoyad ?? string.Empty,
                ["accountInfo"] = accountInfo,
                ["description"] = "Withdrawal request",
                // Callback'te kaydı bulmak için talep Id'si gönderilir. Aynı talep yeniden yönlendirilebildiği
                // için sonuna zaman eklenir (BluePara aynı merchant_transaction_id'yi 409 ile reddediyor).
                ["merchant_transaction_id"] = $"{cekimTalepItem.Id:N}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                ["callbackUrl"] = settings.BlueParaCallbackUrl,
            };

            if (!string.IsNullOrWhiteSpace(cekimTalepItem.UserId))
                payload["merchant_user_id"] = cekimTalepItem.UserId;

            // İki giriş yöntemi var: Provider Key verildiyse eski yöntem (header'da X-PROVIDER-KEY, hash yok),
            // yoksa yeni yöntem (Merchant Key ile hesaplanan hash gövdede).
            var useProviderKey = !string.IsNullOrWhiteSpace(settings.BlueParaProviderKey);
            if (!useProviderKey)
                payload["hash"] = BlueParaSigner.Sign(BlueParaSigner.Stringify(payload), settings.BlueParaMerchantKey);

            var body = BlueParaSigner.Stringify(payload);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.BlueParaBaseUrl.TrimEnd('/')}/merchant-api/withdrawals");
                request.Headers.Add("X-API-KEY", settings.BlueParaApiKey.Trim());
                if (useProviderKey)
                    request.Headers.Add("X-PROVIDER-KEY", settings.BlueParaProviderKey.Trim());
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                BlueParaWithdrawResponse? result = null;
                try
                {
                    result = JsonConvert.DeserializeObject<BlueParaWithdrawResponse>(json);
                }
                catch
                {
                }

                if (response.IsSuccessStatusCode && result != null && result.Success)
                {
                    result.IsSuccess = true;
                    return result;
                }

                var error = result?.Message;
                if (result?.Errors?.Count > 0)
                    error = $"{error} ({string.Join(", ", result.Errors.Select(e => $"{e.Field}: {e.Message}"))})";

                return new BlueParaWithdrawResponse
                {
                    IsSuccess = false,
                    Message = string.IsNullOrWhiteSpace(error) ? $"HTTP {(int)response.StatusCode} {json}" : error
                };
            }
            catch (Exception ex)
            {
                return new BlueParaWithdrawResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
