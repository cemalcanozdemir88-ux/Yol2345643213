using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IFluxFinansDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(FluxFinansCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
