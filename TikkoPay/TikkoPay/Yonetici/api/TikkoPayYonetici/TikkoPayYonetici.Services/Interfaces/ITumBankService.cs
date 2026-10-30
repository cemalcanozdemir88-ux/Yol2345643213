using TikkoPayYonetici.Core.DTOs.TumBank;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface ITumBankService
    {
        Task<TumBankWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
