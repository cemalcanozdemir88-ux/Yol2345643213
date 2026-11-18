using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IBlueParaWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(int status, decimal amount, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
