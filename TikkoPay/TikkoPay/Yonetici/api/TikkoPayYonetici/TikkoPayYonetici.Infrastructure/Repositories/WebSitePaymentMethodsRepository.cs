using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.WebSitePaymentMethods;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class WebSitePaymentMethodsRepository : GenericRepository<WebSitePaymentMethods>, IWebSitePaymentMethodsRepository
    {
        private readonly DbSet<WebSitePaymentMethods> _dbSet;

        public WebSitePaymentMethodsRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<WebSitePaymentMethods>();
        }

        public async Task<IEnumerable<WebSitePaymentMethodsDto>> GetByWebSiteIdDtoAsync(Guid webSiteId)
        {
            var query = _dbSet
                        .Include(wpm => wpm.PaymentMethods)
                        .Where(wpm => wpm.WebSiteleriId.Equals(webSiteId) && wpm.SilindiMi == false);

            return await query.Select(h => new WebSitePaymentMethodsDto
            {
                Id = h.Id,
                WebSiteleriId = h.WebSiteleriId,
                PaymentMethodId = h.PaymentMethodId,
                PaymentMethodBaslik = h.PaymentMethods != null ? h.PaymentMethods.Baslik : string.Empty
            }).ToListAsync();
        }
    }
}
