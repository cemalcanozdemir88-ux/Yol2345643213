using TikkoPayYonetici.Core.DTOs.ParoPay;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IParoPayService
    {
        Task<ParoPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
