using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Infrastructure.Repositories
{
    public class KullaniciOtpValuesRepository : IKullaniciOtpValuesRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<KullaniciOtpValues> _dbSet;

        public KullaniciOtpValuesRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<KullaniciOtpValues>();
        }

        public async Task<KullaniciOtpValues> GetById(Guid Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public async Task<Guid> Add(Guid KullaniciId)
        {
            var item = new KullaniciOtpValues
            {
                Id = Guid.NewGuid(),
                KullaniciId = KullaniciId,
                SilindiMi = false,
                KayitTarihi = DateTime.Now
            };

            var itemsToRemove = _dbSet.Where(k => k.KullaniciId == KullaniciId).ToList();
            _dbSet.RemoveRange(itemsToRemove);

            _dbSet.Add(item);
            await _dbContext.SaveChangesAsync();

            return item.Id;
        }
    }
}
