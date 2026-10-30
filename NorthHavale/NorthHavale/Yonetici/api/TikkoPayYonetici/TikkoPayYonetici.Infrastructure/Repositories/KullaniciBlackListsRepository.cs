using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.KullaniciBlackList;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class KullaniciBlackListsRepository : GenericRepository<KullaniciBlackLists>, IKullaniciBlackListsRepository
    {
        public KullaniciBlackListsRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<int> GetCountAsync(Guid? websiteId, long? userId)
        {
            var query = base._dbContext.Set<KullaniciBlackLists>()
                            .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, websiteId, userId);
            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<KullaniciBlackListDto>> GetAllDtoAsync(Guid? websiteId, long? userId, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = base._dbContext.Set<KullaniciBlackLists>()
                            .Include(k => k.WebSiteleri)
                            .Where(k => k.SilindiMi == false);


            query = ApplyFilters(query, websiteId, userId);

            query = query.Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new KullaniciBlackListDto
            {
                Id = k.Id,
                WebSiteId = k.WebSiteId,
                WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                UserId = k.UserId,
            }).ToListAsync();
        }

        private IQueryable<KullaniciBlackLists> ApplyFilters(IQueryable<KullaniciBlackLists> query, Guid? websiteId, long? userId)
        {
            if (websiteId is not null)
            {
                query = query.Where(k => k.WebSiteId == websiteId);
            }

            if (userId is not null)
            {
                query = query.Where(k => k.UserId == userId);
            }

            return query;
        }
    }
}
