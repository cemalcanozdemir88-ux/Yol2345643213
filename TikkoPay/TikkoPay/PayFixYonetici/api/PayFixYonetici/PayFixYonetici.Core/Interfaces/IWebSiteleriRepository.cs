using PayFixYonetici.Core.DTOs.WebSiteleri;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IWebSiteleriRepository : IGenericRepository<WebSiteleri>
    {
        Task<List<WebSiteleriSelectListDto>> GetSelectListAsync(Guid? webSiteId);
        Task<List<WebSiteleriSelectListDto>> GetSelectLisByAffiliateAsync(Guid affiliateId);
        Task<List<WebSiteleriListDto>> GetAllAsync(Guid? webSiteId);
        Task<List<WebSiteBakiyeListDto>> GetBakiyeList(Guid? webSiteId);
    }
}
