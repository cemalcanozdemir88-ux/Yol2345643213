using TikkoPayYonetici.Core.DTOs.PullPay;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IPullPayService
    {
        Task<PullPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
