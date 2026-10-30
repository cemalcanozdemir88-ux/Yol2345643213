using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.PaymentMethods;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class PaymentMethodsRepository :IPaymentMethodsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<PaymentMethods> _dbSet;

        public PaymentMethodsRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<PaymentMethods>();
        }

        public async Task<IEnumerable<PaymentMethodsDto>> GetAllDtoAsync()
        {
            var query = _dbSet.Where(k => k.SilindiMi == false).OrderBy(k => k.Baslik);

            return await query.Select(k => new PaymentMethodsDto
            {
                Id = k.Id,
                Baslik = k.Baslik,
                PaymentId = k.PaymentId,
                DepositCallbackUrl = k.DepositCallbackUrl,
                WithdrawCallbackUrl = k.WithdrawCallbackUrl
            }).ToListAsync();
        }
    }
}
