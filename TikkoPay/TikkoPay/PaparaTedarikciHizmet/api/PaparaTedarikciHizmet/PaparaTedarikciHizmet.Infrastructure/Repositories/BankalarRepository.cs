using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;

namespace PaparaTedarikciHizmet.Infrastructure.Repositories
{
    public class BankalarRepository : GenericRepository<Bankalar>, IBankalarRepository
    {
        public BankalarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
