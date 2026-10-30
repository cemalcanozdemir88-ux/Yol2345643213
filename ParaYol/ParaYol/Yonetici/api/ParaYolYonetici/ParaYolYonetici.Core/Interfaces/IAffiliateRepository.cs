using ParaYolYonetici.Core.DTOs.Affiliate;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IAffiliateRepository : IGenericRepository<Affiliates>
    {
        Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId);
    }
}
