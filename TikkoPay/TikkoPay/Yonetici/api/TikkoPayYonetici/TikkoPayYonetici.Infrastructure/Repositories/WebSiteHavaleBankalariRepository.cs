using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class WebSiteHavaleBankalariRepository : GenericRepository<WebSiteHavaleBankalari>, IWebSiteHavaleBankalariRepository
    {
        public WebSiteHavaleBankalariRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<WebSiteHavaleBankalariDto>> GetByWebSiteIdAsync(Guid webSiteId)
        {
            return await _dbContext.Set<WebSiteHavaleBankalari>()
                                   .Where(k => k.SilindiMi == false)
                                   .Where(k => k.WebSiteId == webSiteId)
                                   .Where(k => k.HavaleBankalari.SilindiMi == false)
                                   .OrderBy(k => k.HavaleBankalari.Tedarikciler.Unvan)
                                   .ThenBy(k => k.HavaleBankalari.HesapMinTutar)
                                   .Select(k => new WebSiteHavaleBankalariDto
                                   {
                                       Id = k.Id,
                                       WebSiteId = k.WebSiteId,
                                       HavaleBankaId = k.HavaleBankaId,
                                       TedarikciId = k.HavaleBankalari.TedarikciId,
                                       TedarikciBaslik = k.HavaleBankalari.Tedarikciler.Unvan ?? string.Empty,
                                       BankaBaslik = k.HavaleBankalari.Bankalar.Baslik ?? string.Empty,
                                       HesapSahibi = k.HavaleBankalari.HesapSahibi ?? string.Empty,
                                       HesapNo = k.HavaleBankalari.HesapNo ?? string.Empty,
                                       HesapMinTutar = k.HavaleBankalari.HesapMinTutar,
                                       HesapMaxTutar = k.HavaleBankalari.HesapMaxTutar,
                                       Durum = k.HavaleBankalari.Durum,
                                   })
                                   .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid webSiteId, Guid havaleBankaId)
        {
            return await _dbContext.Set<WebSiteHavaleBankalari>()
                                   .AnyAsync(k => k.SilindiMi == false && k.WebSiteId == webSiteId && k.HavaleBankaId == havaleBankaId);
        }
    }
}
