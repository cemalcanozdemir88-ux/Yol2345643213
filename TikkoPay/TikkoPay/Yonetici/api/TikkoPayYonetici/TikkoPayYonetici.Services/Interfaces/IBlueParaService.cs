using TikkoPayYonetici.Core.DTOs.BluePara;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IBlueParaService
    {
        Task<BlueParaWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
