using TikkoPayCallbacks.Infrastructure.Dtos;

namespace TikkoPayCallbacks.Infrastructure.Repositories.Interfaces
{
    public interface ITedarikcilerRepository
    {
        Task<decimal> GetYatirimKomisyonu(Guid tedarikciId, decimal tutar);
        Task ResetHesapNoGosterimSayisi();
        Task<IEnumerable<TedarikciHavaleBankalariDto>> GetTedarikciHavaleBankalari();
        Task<IEnumerable<TedarikciDto>> GetTedarikciler();
        Task<decimal> GetTedarikciBakiye(Guid tedarikciId);
    }
}
