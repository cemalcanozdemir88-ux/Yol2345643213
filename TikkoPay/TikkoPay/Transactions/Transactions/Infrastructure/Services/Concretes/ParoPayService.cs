using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.ParoPay;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class ParoPayService : IParoPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public ParoPayService(HttpClient httpClient, 
                              ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<ParoPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var payload = new
            {
                command = "pay",
                transaction_id = depositTransaction.TransactionId,
                user_id = depositTransaction.UserId,
                user_name = depositTransaction.FullName,
                amount = amount,
                hash = CreateMD5Hash($"pay{depositTransaction.UserId}{depositTransaction.TransactionId}{amount}{settings.ParoPayApiSecret}")
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", settings.ParoPayApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-API-Secret", settings.ParoPayApiSecret);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            var response = await _httpClient.PostAsync(settings.ParoPayDepositUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<ParoPayBankAccountResponse>(json);

                return bankAccountResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Banka hesap bilgileri alma işlemi başarısız: TransactionId: {depositTransaction.TransactionId} - {error}");
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
