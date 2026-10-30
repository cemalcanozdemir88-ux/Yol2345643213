using PayCoYonetici.Core.DTOs.WebSiteleri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IWebSiteleriRepository : IGenericRepository<WebSiteleri>
    {
        Task<List<WebSiteleriSelectListDto>> GetSelectListAsync(Guid? webSiteId);
        Task<List<WebSiteleriSelectListDto>> GetSelectLisByAffiliateAsync(Guid affiliateId);
        Task<List<WebSiteleriListDto>> GetAllAsync(Guid? webSiteId);
        Task<List<WebSiteBakiyeListDto>> GetBakiyeList(Guid? webSiteId);
    }
}
