using TikkoPayYonetici.Core.DTOs.AsPay;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IAsPayServices
    {
        Task<AsPayWithdrawResponse> SendWithdrawToAsPayAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
