using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.ParoPay;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class ParoPayService : IParoPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public ParoPayService(HttpClient httpClient,
                              ISettingsV2Repository settingsV2Repository,
                              IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<ParoPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ParoPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            var amountString = cekimTalepItem.Tutar.ToString("0.####", CultureInfo.InvariantCulture);

            var payloadDict = new SortedDictionary<string, object>
            {
                { "command", "withdraw" },
                { "user_id", cekimTalepItem.UserId },
                { "user_name", cekimTalepItem.AdSoyad },
                { "transaction_id", cekimTalepItem.WebSiteTransactionId },
                { "amount", amountString },
                { "iban", cekimTalepItem.HesapNo },
                { "bank_name", "Fast" },
                { "account_name", cekimTalepItem.HesapSahibi },
                { "hash", CreateMD5Hash($"withdraw{cekimTalepItem.UserId}{cekimTalepItem.WebSiteTransactionId}{amountString}{cekimTalepItem.HesapNo}Fast{cekimTalepItem.HesapSahibi}{settings.ParoPayApiSecret}")}
            };

            var jsonPayload = JsonConvert.SerializeObject(payloadDict);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", settings.ParoPayApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-API-Secret", settings.ParoPayApiSecret);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            var response = await _httpClient.PostAsync(settings.ParoPayWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<ParoPayWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Success;

                return postResponse;
            }
            else
            {
                return new ParoPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse!.Message}",
                };
            }
        }

        private string CreateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
