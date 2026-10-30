using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaycoStartDepositResponse> StartDepositAsync(string apiUrl, PaycoStartDepositRequest request);
        Task<PaycoWithDrawResponse> MakeWithdrawAsync(string apiUrl, PaycoWithDrawRequest request);
    }
}
