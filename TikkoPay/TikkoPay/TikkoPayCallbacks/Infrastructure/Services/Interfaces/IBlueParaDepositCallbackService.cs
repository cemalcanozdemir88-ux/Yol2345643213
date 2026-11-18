using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IBlueParaDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(int status, decimal amount, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
