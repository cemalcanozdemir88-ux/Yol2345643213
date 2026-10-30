using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.WebSiteleri;

namespace PayCoYonetici.Services.Interfaces
{
    public interface IWebSiteleriService
    {
        ResponseViewModel<WebSiteleriDto> GetById(Guid id);
        Task<ResponseViewModel<List<WebSiteleriSelectListDto>>> GetSelectListAsync(Guid? webSiteId);
        Task<ResponseViewModel<List<WebSiteleriSelectListDto>>> GetSelectListByAffiliateAsync(Guid affiliateId);
        Task<ResponseViewModel<List<WebSiteleriListDto>>> GetAllAsync(Guid? webSiteId);
        Task<ResponseViewModel<List<WebSiteBakiyeListDto>>> GetBakiyeList(Guid? webSiteId);
        Task<ResponseViewModel<WebSiteleriDto>> Create(WebSiteleriCreateDto item);
        Task<ResponseViewModel<WebSiteleriDto>> Update(WebSiteleriUpdateDto itemDto);
        Task<ResponseViewModel<WebSiteleriDto>> Delete(Guid id);
    }
}
