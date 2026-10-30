using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.Affiliate;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IAffiliateService
    {
        ResponseViewModel<AffiliateDto> GetById(Guid id);
        Task<ResponseViewModel<List<AffiliateBakiyeListDto>>> GetBakiyeList(Guid? affiliateId, Guid? websiteId);
        Task<ResponseViewModel<List<AffiliateDto>>> GetAllAsync(Guid? affiliateId);
        Task<ResponseViewModel<AffiliateDto>> Delete(Guid id);
        Task<ResponseViewModel<AffiliateDto>> Create(AffiliateCreateDto item);
        Task<ResponseViewModel<AffiliateDto>> Update(AffiliateUpdateDto itemDto);
    }
}
