using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using Microsoft.AspNetCore.Http;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class BankalarRepository : GenericRepository<Bankalar>, IBankalarRepository
    {
        public BankalarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
