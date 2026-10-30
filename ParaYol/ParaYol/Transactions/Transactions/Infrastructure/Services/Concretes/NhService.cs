using System.Text;
using Newtonsoft.Json;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models.Nh;

namespace Transactions.Infrastructure.Services.Concretes
{
    public class NhService : INhService
    {
        private readonly HttpClient _httpClient;
        private ISettingsRepository _settingsRepository;
        private IGeneralHelper _generalHelper;

        public NhService(HttpClient httpClient, ISettingsRepository settingsRepository, IGeneralHelper generalHelper)
        {
            _httpClient = httpClient;
            _settingsRepository = settingsRepository;
            _generalHelper = generalHelper;
        }

        public async Task<NhBankAccountResponse> GetBankAccountAsync(DepositTransaction depositTransaction, decimal amount)
        {
            var settings = await _settingsRepository.GetFirst();

            var hash = _generalHelper.GenerateHashForNh(depositTransaction.TransactionId, depositTransaction.UserId, settings.NhSiteClientId, settings.NhSiteSecretId);

            var payload = new
            {
                clientid = settings.NhSiteClientId,
                clientsecret = settings.NhSiteSecretId,
                type = "deposit",
                transactionId = depositTransaction.TransactionId,
                userid = depositTransaction.UserId,
                fullname = depositTransaction.FullName,
                username = depositTransaction.UserName,
                amount = amount,
                hash = hash
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(settings.NhDepositUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankAccountResponse = JsonConvert.DeserializeObject<NhBankAccountResponse>(json);

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
