using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IPullPayWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(PullPayCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
