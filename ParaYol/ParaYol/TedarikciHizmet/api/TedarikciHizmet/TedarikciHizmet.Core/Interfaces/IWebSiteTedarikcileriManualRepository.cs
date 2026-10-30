using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IWebSiteTedarikcileriManualRepository : IGenericRepository<WebSiteTedarikcileriManual>
    {
        Task<List<WebSiteTedarikcileriManual>> GetByTedarikciIdAsync(Guid tedarikciId);
    }
}
