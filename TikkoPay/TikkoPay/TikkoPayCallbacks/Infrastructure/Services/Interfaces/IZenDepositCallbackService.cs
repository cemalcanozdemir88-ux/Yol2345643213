using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IZenDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(ZenDepositCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
