using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TikkoPayYonetici.Core.DTOs.Zen;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class ZenService : IZenService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;

        public ZenService(HttpClient httpClient, 
                          ISettingsV2Repository settingsV2Repository, 
                          IWithdrawTransactionRepository withdrawTransactionRepository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
        }

        public async Task<ZenWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem)
        {
            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(cekimTalepItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ZenWithdrawResponse
                {
                    IsSuccess = false,
                    Message = "WithdrawTransaction not found"
                };
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            var amountString = cekimTalepItem.Tutar.ToString("0.####", CultureInfo.InvariantCulture);
            
            string hashValue = CreateMd5($"{settings.ZenEntegreMid}{amountString}transfer{settings.ZenEntegreSecret}");

            var url = $"{settings.ZenEntegreWithdrawUrl}/withdraw?" +
                $"mid={settings.ZenEntegreMid}" +
                $"&hash={hashValue}" +
                $"&action=start" +
                $"&method=transfer" +
                $"&usercode={cekimTalepItem.UserId}" +
                $"&username={cekimTalepItem.UserName}" +
                $"&amount={amountString}" +
                $"&currency=TL" +
                $"&name={cekimTalepItem.HesapSahibi}" +
                $"&account={cekimTalepItem.HesapNo}" +
                $"&transactionId={cekimTalepItem.WebSiteTransactionId}" +
                $"&callbackUrl=https://callback.payorapay.com/api/zen/withdraw" +
                $"&bankId=32";

            var response = await _httpClient.GetAsync(url);

            var json = await response.Content.ReadAsStringAsync();
            var postResponse = JsonConvert.DeserializeObject<ZenWithdrawResponse>(json);

            if (response.IsSuccessStatusCode)
            {
                postResponse!.IsSuccess = true;

                return postResponse;
            }
            else
            {
                return new ZenWithdrawResponse
                {
                    IsSuccess = false,
                    Message = $"{postResponse!.Message}",
                };
            }
        }

        private string CreateMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // hex format
                }

                return sb.ToString();
            }
        }
    }
}
