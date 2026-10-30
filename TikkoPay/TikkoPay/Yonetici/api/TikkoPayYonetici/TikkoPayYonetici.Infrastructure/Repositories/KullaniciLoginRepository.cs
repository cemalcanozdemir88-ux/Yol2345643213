using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class KullaniciLoginRepository : IKullaniciLoginRepository
    {
        protected readonly AppDbContext _dbContext;

        public KullaniciLoginRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(KullaniciLogin entity)
        {
            entity.KayitTarihi = DateTime.Now;
            await _dbContext.Set<KullaniciLogin>().AddAsync(entity);

            await _dbContext.SaveChangesAsync();
        }
    }
}
