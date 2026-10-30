using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteMutabakatlari;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSiteMutabakatService
    {
        Task<ResponseViewModel<List<WebSiteMutabakatlariListDto>>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi);
        Task<ResponseViewModel<WebSiteMutabakatlariListDto>> Calculate(Guid webSiteId, decimal devir, DateOnly mutabakatTarihi);
    }
}
