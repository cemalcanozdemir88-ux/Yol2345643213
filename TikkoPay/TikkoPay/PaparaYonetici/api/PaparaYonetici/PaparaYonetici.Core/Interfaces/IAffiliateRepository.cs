using PaparaYonetici.Core.DTOs.Affiliate;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IAffiliateRepository : IGenericRepository<Affiliates>
    {
        Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId);
    }
}
