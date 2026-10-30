using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public class BankalarRepository : GenericRepository<Bankalar>, IBankalarRepository
    {
        public BankalarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
