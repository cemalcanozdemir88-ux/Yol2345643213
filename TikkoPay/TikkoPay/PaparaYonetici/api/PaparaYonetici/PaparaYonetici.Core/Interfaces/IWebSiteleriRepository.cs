using PaparaYonetici.Core.DTOs.WebSiteleri;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IWebSiteleriRepository : IGenericRepository<WebSiteleri>
    {
        Task<List<WebSiteleriSelectListDto>> GetSelectListAsync(Guid? webSiteId);
        Task<List<WebSiteleriSelectListDto>> GetSelectLisByAffiliateAsync(Guid affiliateId);
        Task<List<WebSiteleriListDto>> GetAllAsync(Guid? webSiteId);
        Task<List<WebSiteBakiyeListDto>> GetBakiyeList(Guid? webSiteId);
    }
}
