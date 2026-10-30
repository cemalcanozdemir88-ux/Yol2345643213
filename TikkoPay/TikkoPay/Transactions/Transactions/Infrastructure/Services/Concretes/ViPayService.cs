using System.Text;
using Newtonsoft.Json;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.ViPay;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class ViPayService : IViPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsV2Repository _settingsV2Repository;

        public ViPayService(HttpClient httpClient, ISettingsV2Repository settingsV2Repository)
        {
            _httpClient = httpClient;
            _settingsV2Repository = settingsV2Repository;
        }

        public async Task<ViPayBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var payload = new
            {
                site = settings.ViPaySiteId,
                method = settings.ViPayMethodId,
                type = "deposit",
                @ref = depositTransaction.TransactionId,
                member_id = depositTransaction.UserId,
                full_name = depositTransaction.FullName,
                username = depositTransaction.UserName,
                amount = amount,
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("X-Secret-Key", settings.ViPaySecretKey);

            var response = await _httpClient.PostAsync(settings.ViPayDepositUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<ViPayBankAccountResponse>(json);

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
