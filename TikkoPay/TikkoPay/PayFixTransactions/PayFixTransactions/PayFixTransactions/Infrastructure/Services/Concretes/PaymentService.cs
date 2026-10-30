using PayFixTransactions.Infrastructure.Services.Interfaces;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Services.Concretes
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProvisionResponse> MakeProvisionAsync(string apiUrl, ProvisionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProvisionResponse>();
        }

        public async Task<PayfixWithDrawResponse> MakeWithdrawAsync(string apiUrl, PayfixWithDrawRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PayfixWithDrawResponse>();
        }
    }
}
