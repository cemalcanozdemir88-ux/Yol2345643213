using PayFixYonetici.Core.DTOs.Affiliate;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Interfaces
{
    public interface IAffiliateRepository : IGenericRepository<Affiliates>
    {
        Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId);
    }
}
