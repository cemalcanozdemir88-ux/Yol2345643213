using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.WebSiteleri;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface IWebSiteleriService
    {
        ResponseViewModel<WebSiteleriDto> GetById(Guid id);
        ResponseViewModel<List<WebSiteleriListDto>> GetAll(Guid? webSiteId);
        Task<ResponseViewModel<List<WebSiteBakiyeListDto>>> GetBakiyeList(Guid? webSiteId);
    }
}
