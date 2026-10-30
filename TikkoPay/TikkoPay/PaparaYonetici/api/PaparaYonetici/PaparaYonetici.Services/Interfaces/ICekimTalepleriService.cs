using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;

namespace PaparaYonetici.Services.Interfaces
{
    public interface ICekimTalepleriService
    {
        Task<ResponseViewModel<YatirimTalepleriListDto>> Approve(Guid id);
    }
}
