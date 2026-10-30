using TikkoPayYonetici.Core.Interfaces;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveAsync(bool setBaseModelValues = true)
        {
            return await _dbContext.SaveChangesAsync(setBaseModelValues);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

    }
}
