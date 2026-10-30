using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class WebSiteTedarikcileriManualRepository : GenericRepository<WebSiteTedarikcileriManual>, IWebSiteTedarikcileriManualRepository
    {
        public WebSiteTedarikcileriManualRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<WebSiteTedarikcileriManualDto>> GetByWebSiteIdAsync(Guid webSiteId)
        {
            var query = _dbContext.Set<WebSiteTedarikcileriManual>()
                                  .Include(k => k.Tedarikciler)
                                  .Include(k => k.WebSiteleri)
                                  .Where(k => k.SilindiMi == false)
                                  .Where(k => k.WebSiteId == webSiteId);

            var result = await query.Select(k => new WebSiteTedarikcileriManualDto
            {
                Id = k.Id,
                WebSiteId = k.WebSiteId,
                TedarikciId = k.TedarikciId,
                WebSiteBaslik = k.WebSiteleri.Baslik ?? string.Empty,
                TedarikciBaslik = k.Tedarikciler.Unvan ?? string.Empty,
            }).ToListAsync();

            return result;
        }
    }
}
