using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteKasaHaraketleri;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IWebSiteKasaHaraketleriService
    {
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModelPagination<List<WebSiteKasaHaraketleriListDto>>> GetAll(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Create(WebSiteKasaHaraketleriCreateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Update(WebSiteKasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Delete(Guid id);
    }
}
