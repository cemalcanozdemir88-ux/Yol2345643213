using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Infrastructure.Services.Interfaces
{
    public interface INhDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(NhCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
