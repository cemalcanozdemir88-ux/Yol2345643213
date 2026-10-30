using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.WebSiteKasaHaraketleri;

namespace PaparaTedarikciHizmet.Services.Interfaces
{
    public interface IWebSiteKasaHaraketleriService
    {
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<WebSiteKasaHaraketleriListDto>>> GetAll(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Create(WebSiteKasaHaraketleriCreateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Update(WebSiteKasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Delete(Guid id);
    }
}
