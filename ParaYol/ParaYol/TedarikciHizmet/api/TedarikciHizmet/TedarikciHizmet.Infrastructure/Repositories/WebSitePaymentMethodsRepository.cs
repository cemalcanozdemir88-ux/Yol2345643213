using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class WebSitePaymentMethodsRepository : GenericRepository<WebSitePaymentMethods>, IWebSitePaymentMethodsRepository
    {
        private readonly DbSet<WebSitePaymentMethods> _dbSet;

        public WebSitePaymentMethodsRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<WebSitePaymentMethods>();
        }
    }
}
