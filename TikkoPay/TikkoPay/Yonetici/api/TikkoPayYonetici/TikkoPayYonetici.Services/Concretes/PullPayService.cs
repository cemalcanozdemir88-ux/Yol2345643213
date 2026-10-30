using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.PullPay;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class PullPayService : IPullPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public PullPayService(HttpClient httpClient, 
                              ISettingsV2Repository settingsV2Repository, 
                              IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<PullPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new PullPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            var payloadDict = new SortedDictionary<string, object>
            {
                { "transaction_id", cekimTalepItem.WebSiteTransactionId },
                { "receiver_holder_name", cekimTalepItem.HesapSahibi },
                { "receiver_iban", cekimTalepItem.HesapNo },
                { "userdata", cekimTalepItem.UserId  },
                { "amount", cekimTalepItem.Tutar },
                { "callback_url", settings.PullPayCallbackUrl },
                { "website_id", settings.PullPayWebsiteId },
            };

            var jsonPayload = JsonConvert.SerializeObject(payloadDict);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("email", settings.PullPayEmail);
            _httpClient.DefaultRequestHeaders.Add("password", settings.PullPayPassword);

            var response = await _httpClient.PostAsync(settings.PullPayWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<PullPayWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Status;

                return postResponse;
            }
            else
            {
                return new PullPayWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse.Message}",
                };
            }
        }
    }
}
