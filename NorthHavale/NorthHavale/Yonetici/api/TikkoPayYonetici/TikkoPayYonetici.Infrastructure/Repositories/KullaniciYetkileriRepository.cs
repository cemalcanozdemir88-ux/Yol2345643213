using TikkoPayYonetici.Core.DTOs.KullaniciYetkileri;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class KullaniciYetkileriRepository : GenericRepository<KullaniciYetkileri>, IKullaniciYetkileriRepository
    {
        private readonly DbSet<KullaniciYetkileri> _dbSetKullaniciYetkileri;
        private readonly DbSet<Yetkiler> _dbSetYetkiler;

        public KullaniciYetkileriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSetKullaniciYetkileri = _dbContext.Set<KullaniciYetkileri>();
            _dbSetYetkiler = _dbContext.Set<Yetkiler>();
        }

        public async Task<IEnumerable<KullaniciYetkileriListDto>> GetByKullaniciId(Guid kullaniciId)
        {
            return await _dbSetYetkiler.Where(k => k.SilindiMi == false)
                 .Select(yetki => new KullaniciYetkileriListDto
                 {
                     KullaniciId = kullaniciId,
                     YetkiId = yetki.Id,
                     UstId = yetki.UstId,
                     SiraNumarasi = yetki.SiraNumarasi,
                     Baslik = yetki.Baslik,
                     KullaniciYetkiliMi = _dbSetKullaniciYetkileri
                                             .Any(ky => ky.KullaniciId == kullaniciId &&
                                                        ky.YetkiId == yetki.Id &&
                                                        ky.SilindiMi == false)
                 })
                 .ToListAsync();

            
        }

        public async Task<bool> KullaniciYetkiVarMi(Guid kullaniciId, List<Guid> yetkiIds)
        {
            return await _dbSetKullaniciYetkileri.AnyAsync(k => k.SilindiMi == false &&
                                                             k.KullaniciId == kullaniciId &&
                                                             yetkiIds.Contains(k.YetkiId)
                                                           );
                                        
        }
    }
}
