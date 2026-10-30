using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Core.Models;
using Microsoft.AspNetCore.Http;

namespace PaparaYonetici.Infrastructure.Repositories
{
    public class BankalarRepository : GenericRepository<Bankalar>, IBankalarRepository
    {
        public BankalarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
