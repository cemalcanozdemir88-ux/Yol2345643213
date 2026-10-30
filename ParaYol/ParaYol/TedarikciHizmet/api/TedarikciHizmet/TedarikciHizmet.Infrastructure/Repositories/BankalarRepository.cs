using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class BankalarRepository : GenericRepository<Bankalar>, IBankalarRepository
    {
        public BankalarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
