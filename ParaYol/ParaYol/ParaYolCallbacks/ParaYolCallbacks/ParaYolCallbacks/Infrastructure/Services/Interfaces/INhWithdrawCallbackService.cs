using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Infrastructure.Services.Interfaces
{
    public interface INhWithdrawCallbackService
    {
        Task<ProcessResultModel> ProcessWithdraw(NhCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings);
    }
}
