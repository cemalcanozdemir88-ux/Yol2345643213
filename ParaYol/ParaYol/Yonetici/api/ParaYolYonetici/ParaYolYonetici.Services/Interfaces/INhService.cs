using ParaYolYonetici.Core.DTOs.Nh;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface INhService
    {
        Task<NhWithdrawResponse> SendWithdrawAsync(YatirimCekimTalepleri cekimTalepItem);
    }
}
