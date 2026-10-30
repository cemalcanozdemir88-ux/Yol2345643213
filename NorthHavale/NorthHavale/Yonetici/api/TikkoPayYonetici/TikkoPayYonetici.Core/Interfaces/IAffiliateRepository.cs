using TikkoPayYonetici.Core.DTOs.Affiliate;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IAffiliateRepository : IGenericRepository<Affiliates>
    {
        Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId);
    }
}
