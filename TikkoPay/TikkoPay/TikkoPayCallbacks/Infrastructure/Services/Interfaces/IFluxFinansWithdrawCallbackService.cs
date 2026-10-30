using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IFluxFinansWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(FluxFinansCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
