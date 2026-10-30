using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Interfaces
{
    public interface IParoPayWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(ParoPayCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
