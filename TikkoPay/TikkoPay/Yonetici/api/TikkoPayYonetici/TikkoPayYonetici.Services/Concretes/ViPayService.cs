using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.ViPay;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class ViPayService : IViPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public ViPayService(HttpClient httpClient, 
                           ISettingsV2Repository settingsV2Repository, 
                           IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
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

            var settings = await _settingsV2Repository.GetSettingsAsync();

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
