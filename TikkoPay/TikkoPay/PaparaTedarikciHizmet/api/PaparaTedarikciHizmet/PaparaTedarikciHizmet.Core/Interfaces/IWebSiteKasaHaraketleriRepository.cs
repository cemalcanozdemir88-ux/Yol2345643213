using PaparaTedarikciHizmet.Core.DTOs.WebSiteKasaHaraketleri;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Interfaces
{
    public interface IWebSiteKasaHaraketleriRepository : IGenericRepository<WebSiteKasaHaraketleri>
    {
        Task<WebSiteKasaHaraketleriListDto> GetByIdDtoAsync(Guid id);
        Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
