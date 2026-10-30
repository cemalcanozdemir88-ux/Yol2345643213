using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IViPayWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(ViPayCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
