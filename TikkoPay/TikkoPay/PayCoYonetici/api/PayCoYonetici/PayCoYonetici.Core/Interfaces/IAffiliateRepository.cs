using PayCoYonetici.Core.DTOs.Affiliate;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Interfaces
{
    public interface IAffiliateRepository : IGenericRepository<Affiliates>
    {
        Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId);
    }
}
