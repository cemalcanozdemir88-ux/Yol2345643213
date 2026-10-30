using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteMutabakatlari;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IWebSiteMutabakatService
    {
        Task<ResponseViewModel<List<WebSiteMutabakatlariListDto>>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi);
        Task<ResponseViewModel<WebSiteMutabakatlariListDto>> Calculate(Guid webSiteId, decimal devir, DateOnly mutabakatTarihi);
    }
}
