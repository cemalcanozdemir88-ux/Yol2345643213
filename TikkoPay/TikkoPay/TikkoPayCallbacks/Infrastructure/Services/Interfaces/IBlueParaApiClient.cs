using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IBlueParaApiClient
    {
        Task<BlueParaTransactionInfo?> GetDepositAsync(string transactionId, Settings settings);
        Task<BlueParaTransactionInfo?> GetWithdrawalAsync(string reference, Settings settings);
    }
}
