using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IWebSiteKasaHaraketleriService
    {
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> GetById(Guid id);
        Task<ResponseViewModelPagination<List<WebSiteKasaHaraketleriListDto>>> GetAll(Guid? webSiteId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Create(WebSiteKasaHaraketleriCreateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> CreateVirman(WebSiteKasaHaraketleriVirmanCreateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Update(WebSiteKasaHaraketleriUpdateDto item);
        Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Delete(Guid id);
    }
}
