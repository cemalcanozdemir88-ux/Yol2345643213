using System.Text;
using Newtonsoft.Json;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.TumBank;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class TumBankService : ITumBankService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public TumBankService(HttpClient httpClient, 
                              ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<TumBankBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var payload = new
            {
                api_key = settings.TumBankApiKey,
                api_secret = settings.TumBankApiSecret,
                user_id = depositTransaction.UserId,
                username = depositTransaction.UserName,
                amount = depositTransaction.Amount,
                fullname = depositTransaction.FullName,
                trx = depositTransaction.TransactionId
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(settings.TumBankDepositUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<TumBankBankAccountResponse>(json);

                return bankAccountResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Banka hesap bilgileri alma işlemi başarısız: TransactionId: {depositTransaction.TransactionId} - {error}");
            }
        }
    }
}
