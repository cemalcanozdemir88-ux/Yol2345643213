using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParaYolYonetici.Core.DTOs.WebSiteTedarikcileri;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public class WebSiteTedarikcileriRepository : GenericRepository<WebSiteTedarikcileri>, IWebSiteTedarikcileriRepository
    {
        public WebSiteTedarikcileriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<WebSiteTedarikcileriDto>> GetByWebSiteIdAsync(Guid webSiteId)
        {
            var query = _dbContext.Set<WebSiteTedarikcileri>()
                                  .Include(k => k.Tedarikciler)
                                  .Include(k => k.WebSiteleri)
                                  .Where(k => k.SilindiMi == false)
                                  .Where(k => k.WebSiteId == webSiteId);

            var result = await query.Select(k => new WebSiteTedarikcileriDto
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
