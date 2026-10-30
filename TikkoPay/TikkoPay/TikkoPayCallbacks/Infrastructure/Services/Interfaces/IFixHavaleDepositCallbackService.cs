using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IFixHavaleDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(FixHavaleCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
