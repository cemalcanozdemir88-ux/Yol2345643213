using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace PaparaTedarikciHizmet.Infrastructure.Repositories
{
    public class WithdrawTransactionRepository : IWithdrawTransactionRepository
    {
        protected readonly AppDbContext _dbContext;

        public WithdrawTransactionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbContext.Set<WithdrawTransaction>().FirstOrDefaultAsync(k => k.TransactionId == transactionId);
        }
    }
}
