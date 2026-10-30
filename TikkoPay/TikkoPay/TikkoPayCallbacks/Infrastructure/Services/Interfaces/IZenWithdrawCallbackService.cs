using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IZenWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(ZenWithdrawCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
