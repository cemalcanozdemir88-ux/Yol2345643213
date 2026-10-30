using TikkoPayYonetici.Core.DTOs.FluxFinans;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IFluxFinansService
    {
        Task<FluxFinansWithdrawResponse> SendWithdrawToFluxFinanceAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
