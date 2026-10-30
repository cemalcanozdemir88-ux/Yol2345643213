using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IPullPayDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(PullPayCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
