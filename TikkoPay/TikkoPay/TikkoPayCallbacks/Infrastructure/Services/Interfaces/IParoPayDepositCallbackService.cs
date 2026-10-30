using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IParoPayDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(ParoPayCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
