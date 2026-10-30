using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface ITumBankDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(TumBankCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
