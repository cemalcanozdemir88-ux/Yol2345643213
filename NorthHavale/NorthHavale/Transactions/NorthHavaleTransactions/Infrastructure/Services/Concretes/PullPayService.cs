using System.Text;
using Newtonsoft.Json;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Infrastructure.Services.Interfaces;
using NorthHavaleTransactions.Models.PullPay;

namespace NorthHavaleTransactions.Infrastructure.Services.Concretes
{
    public class PullPayService : IPullPayService
    {
        private readonly HttpClient _httpClient;
        private ISettingsV2Repository _settingsV2Repository;

        public PullPayService(HttpClient httpClient,
                              ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<PullPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var payload = new
            {
                amount = amount,
                fullname = depositTransaction.FullName,
                userdata = depositTransaction.UserId,
                callback_url = settings.PullPayCallbackUrl,
                transaction_id = depositTransaction.TransactionId,
                website_id = settings.PullPayWebsiteId
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("email", settings.PullPayEmail);
            _httpClient.DefaultRequestHeaders.Add("password", settings.PullPayPassword);

            var response = await _httpClient.PostAsync(settings.PullPayDepositUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<PullPayBankAccountResponse>(json);

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
