using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.WebSiteleri;

namespace PaparaTedarikciHizmet.Services.Interfaces
{
    public interface IWebSiteleriService
    {
        ResponseViewModel<WebSiteleriDto> GetById(Guid id);
        ResponseViewModel<List<WebSiteleriListDto>> GetAll(Guid? webSiteId);
        Task<ResponseViewModel<List<WebSiteBakiyeListDto>>> GetBakiyeList(Guid? webSiteId);
    }
}
