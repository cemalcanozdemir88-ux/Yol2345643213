using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Infrastructure.Services.Interfaces
{
    public interface IViPayWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(ViPayCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
