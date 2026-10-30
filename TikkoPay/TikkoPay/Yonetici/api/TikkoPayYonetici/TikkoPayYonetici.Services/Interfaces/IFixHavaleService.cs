using TikkoPayYonetici.Core.DTOs.FixHavale;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IFixHavaleService
    {
        Task<FixHavaleWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
