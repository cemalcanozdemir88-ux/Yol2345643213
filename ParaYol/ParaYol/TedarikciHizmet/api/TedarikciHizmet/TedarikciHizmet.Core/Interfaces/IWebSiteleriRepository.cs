using TedarikciHizmet.Core.DTOs.WebSiteleri;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IWebSiteleriRepository : IGenericRepository<WebSiteleri>
    {
        Task<List<WebSiteBakiyeListDto>> GetBakiyeList(Guid? webSiteId);
    }
}
