using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IViPayDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(ViPayCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
