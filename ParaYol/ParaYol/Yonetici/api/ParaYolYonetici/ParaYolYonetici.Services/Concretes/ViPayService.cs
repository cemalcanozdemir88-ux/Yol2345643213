using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ParaYolYonetici.Core.DTOs.ViPay;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class ViPayService : IViPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public ViPayService(HttpClient httpClient, 
                           ISettingsRepository settingsRepository, 
                           IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsRepository = settingsRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<ViPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ViPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsRepository.GetFirst();

            var payloadDict = new SortedDictionary<string, object>
            {
                { "amount", cekimTalepItem.Tutar },
                { "site", settings.ViPaySiteId },
                { "secret_key", settings.ViPaySecretKey },
                { "ref", cekimTalepItem.WebSiteTransactionId },
                { "method", settings.ViPayMethodId },
                { "username", cekimTalepItem.UserName },
                { "type", "withdrawal" },
                { "customer_iban", cekimTalepItem.HesapNo },
                { "member_id", cekimTalepItem.UserId },
                { "transaction_note", "Withdrawal request" },
                { "full_name", cekimTalepItem.AdSoyad }
            };

            string signature = GenerateHmacSignature(payloadDict, settings.ViPaySignKey);

            payloadDict.Add("signature", signature);

            var jsonPayload = JsonConvert.SerializeObject(payloadDict);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Secret-Key", settings.ViPaySecretKey);
            var response = await _httpClient.PostAsync(settings.ViPayWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<ViPayWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Success;

                return postResponse;
            }
            else
            {
                return new ViPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse.Error}",
                };
            }
        }

        private string GenerateHmacSignature(object payload, string signingSecret)
        {
            var dictionary = JsonConvert.DeserializeObject<SortedDictionary<string, object>>(
                JsonConvert.SerializeObject(payload)
            );

            string compactJson = JsonConvert.SerializeObject(dictionary, Formatting.None);

            byte[] keyBytes = Encoding.UTF8.GetBytes(signingSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(compactJson);

            using (var hmac = new HMACSHA1(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
