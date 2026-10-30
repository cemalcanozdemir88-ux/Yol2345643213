using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Infrastructure.Services.Interfaces
{
    public interface IViPayDepositCallbackService
    {
        Task<ProcessResultModel> ProcessDeposit(ViPayCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings);
    }
}
