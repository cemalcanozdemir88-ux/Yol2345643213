using TikkoPayYonetici.Core.DTOs.ViPay;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IViPayService
    {
        Task<ViPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
