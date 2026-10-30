using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.TumBank;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class TumBankService : ITumBankService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public TumBankService(HttpClient httpClient, 
                              ISettingsV2Repository settingsV2Repository, 
                              IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<TumBankWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new TumBankWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            var amountString = cekimTalepItem.Tutar.ToString("0.####", CultureInfo.InvariantCulture);

            var payloadDict = new SortedDictionary<string, object>
            {
                { "api_key", settings.TumBankApiKey },
                { "api_secret", settings.TumBankApiSecret },
                { "user_id", cekimTalepItem.UserId },
                { "username", cekimTalepItem.UserName },
                { "fullname", cekimTalepItem.HesapSahibi },
                { "trx", cekimTalepItem.WebSiteTransactionId },
                { "amount", amountString },
                { "iban", cekimTalepItem.HesapNo },
                { "method", "BankTransfer" }
            };

            var jsonPayload = JsonConvert.SerializeObject(payloadDict);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(settings.TumBankWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<TumBankWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Status == 1;

                return postResponse;
            }
            else
            {
                return new TumBankWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse!.Message}",
                };
            }
        }
    }
}
