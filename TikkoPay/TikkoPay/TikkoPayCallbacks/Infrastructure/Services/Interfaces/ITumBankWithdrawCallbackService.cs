using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface ITumBankWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(TumBankCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
