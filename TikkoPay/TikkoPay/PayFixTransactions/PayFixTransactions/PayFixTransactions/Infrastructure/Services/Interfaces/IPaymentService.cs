using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ProvisionResponse> MakeProvisionAsync(string apiUrl, ProvisionRequest request);
        Task<PayfixWithDrawResponse> MakeWithdrawAsync(string apiUrl, PayfixWithDrawRequest request);
    }
}
