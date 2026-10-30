using PayCoTransactions.Infrastructure.Services.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Services.Concretes
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaycoStartDepositResponse> StartDepositAsync(string apiUrl, PaycoStartDepositRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaycoStartDepositResponse>();
        }

        public async Task<PaycoWithDrawResponse> MakeWithdrawAsync(string apiUrl, PaycoWithDrawRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaycoWithDrawResponse>();
        }
    }
}
