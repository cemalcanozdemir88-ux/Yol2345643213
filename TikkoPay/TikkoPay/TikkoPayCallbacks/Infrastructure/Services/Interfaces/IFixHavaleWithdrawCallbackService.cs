using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IFixHavaleWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(FixHavaleCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
