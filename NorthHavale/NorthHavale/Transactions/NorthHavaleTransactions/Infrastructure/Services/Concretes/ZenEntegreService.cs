using System.Security.Cryptography;
using System.Text;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Infrastructure.Services.Interfaces;
using NorthHavaleTransactions.Models.ZenEntegre;

namespace NorthHavaleTransactions.Infrastructure.Services.Concretes
{
    public class ZenEntegreService : IZenEntegreService
    {
        private readonly HttpClient _httpClient;
        private ISettingsV2Repository _settingsV2Repository;

        public ZenEntegreService(HttpClient httpClient,
                                 ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<ZenEntegreBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            string hashValue = CreateMd5($"{settings.ZenEntegreMid}{amount}transfer{settings.ZenEntegreSecret}");

            var url = $"{settings.ZenEntegreDepositUrl}/deposit?" +
                $"mid={settings.ZenEntegreMid}" +
                $"&hash={hashValue}" +
                $"&action=start" +
                $"&method=transfer" +
                $"&usercode={depositTransaction.UserId}" +
                $"&username={depositTransaction.UserName}" +
                $"&amount={amount}" +
                $"&currency=TL" +
                $"&name={depositTransaction.FullName}" +
                $"&transactionId={depositTransaction.TransactionId}" +
                $"&callbackUrl=https://callback.payorapay.com/api/zen/deposit" +
                $"&bankId=32";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = System.Text.Json.JsonSerializer.Deserialize<ZenEntegreBankAccountResponse>(json);

                return bankAccountResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Banka hesap bilgileri alma işlemi başarısız: TransactionId: {depositTransaction.TransactionId} - {error}");
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
                    sb.Append(b.ToString("x2")); 
                }

                return sb.ToString();
            }
        }
    }
}
