using ParaYolYonetici.Core.DTOs.ViPay;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IViPayService
    {
        Task<ViPayWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
