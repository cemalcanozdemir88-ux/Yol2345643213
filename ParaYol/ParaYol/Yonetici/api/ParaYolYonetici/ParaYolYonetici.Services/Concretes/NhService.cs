using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ParaYolYonetici.Core.DTOs.Nh;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class NhService : INhService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public NhService(HttpClient httpClient,
                           ISettingsRepository settingsRepository,
                           IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsRepository = settingsRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<NhWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new NhWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsRepository.GetFirst();

            var hash = GenerateHash(cekimTalepItem.WebSiteTransactionId, cekimTalepItem.UserId, settings.NhSiteClientId, settings.NhSiteSecretId);

            var payloadDict = new SortedDictionary<string, object>
            {
                { "username", cekimTalepItem.UserName },
                { "userid", cekimTalepItem.UserId },
                { "transactionid", cekimTalepItem.WebSiteTransactionId },
                { "fullname", cekimTalepItem.AdSoyad },
                { "amount", cekimTalepItem.Tutar },
                { "iban", cekimTalepItem.HesapNo },
                { "clientid", settings.NhSiteClientId },
                { "clientsecret", settings.NhSiteSecretId },
                { "hash", hash },
            };

            var jsonPayload = JsonConvert.SerializeObject(payloadDict);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            var response = await _httpClient.PostAsync(settings.NhWithdrawUrl, content);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<NhWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = postResponse.Success;

                return postResponse;
            }
            else
            {
                return new NhWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse.Message}",
                };
            }
        }

        private string GenerateHash(string TransactionId, string UserId, string ClientId, string ClientSecret)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashInput = $"{TransactionId}{UserId}{ClientId}{ClientSecret}";
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
