using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public class DepositTransactionRepository : IDepositTransactionRepository
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppDbContext _dbContext;

        public DepositTransactionRepository(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<DepositTransaction?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbContext.Set<DepositTransaction>().FirstOrDefaultAsync(k => k.TransactionId == transactionId);
        }
    }
}
