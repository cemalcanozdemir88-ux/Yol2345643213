using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class ActionRequestLogRepository : IActionRequestLogRepository
    {
        protected readonly AppDbContext _dbContext;

        public ActionRequestLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(ActionRequestLog entity)
        {
            entity.KayitTarihi = DateTime.Now;
            await _dbContext.Set<ActionRequestLog>().AddAsync(entity);
        }
    }
}
