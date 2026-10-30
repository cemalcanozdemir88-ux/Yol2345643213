using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class WebSiteTedarikcileriManualRepository : GenericRepository<WebSiteTedarikcileriManual>, IWebSiteTedarikcileriManualRepository
    {
        public WebSiteTedarikcileriManualRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<List<WebSiteTedarikcileriManual>> GetByTedarikciIdAsync(Guid tedarikciId)
        {
            var query = _dbContext.Set<WebSiteTedarikcileriManual>()
                                  .Where(k => k.SilindiMi == false && k.TedarikciId == tedarikciId);
            return await query.ToListAsync();
        }
    }
}
