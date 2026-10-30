using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Repositories
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
