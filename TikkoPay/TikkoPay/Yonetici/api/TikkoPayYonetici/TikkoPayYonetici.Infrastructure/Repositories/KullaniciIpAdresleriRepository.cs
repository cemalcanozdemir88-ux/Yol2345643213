using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class KullaniciIpAdresleriRepository : GenericRepository<KullaniciIpAdresleri>, IKullaniciIpAdresleriRepository
    {
        public KullaniciIpAdresleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<KullaniciIpAdresleriListDto?> GetByIdDtoAsync(Guid id)
        {
            var query = _dbContext.Set<KullaniciIpAdresleri>()
                                  .Include(k => k.Kullanicilar)
                                  .Where(k => k.Id.Equals(id) && k.SilindiMi == false);

            return await query.Select(k => new KullaniciIpAdresleriListDto
            {
                Id = k.Id,
                KullaniciId = k.KullaniciId,
                KullaniciEPosta = k.Kullanicilar.Eposta,
                KullaniciAdSoyad = k.Kullanicilar.AdSoyad,
                KullaniciTipi = k.Kullanicilar.KullaniciTipi,
                IpAdresi = k.IpAdresi,
            })
            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<KullaniciIpAdresleriListDto>> GetAllDtoAsync(Guid? tedarikciId, string searchText)
        {
            var query = _dbContext.Set<KullaniciIpAdresleri>()
                                  .Include(k => k.Kullanicilar)
                                  .Include(k => k.Kullanicilar.Tedarikciler)
                                  .Include(k => k.Kullanicilar.WebSiteleri)
                                  .Where(k => k.SilindiMi == false);

            if (tedarikciId.HasValue)
            {
                query = query.Where(k => k.Kullanicilar.TedarikciId == tedarikciId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => k.IpAdresi.Contains(searchText) ||
                                         k.Kullanicilar.Eposta.Contains(searchText) ||
                                         k.Kullanicilar.AdSoyad.Contains(searchText));
            }


            return await query.Select(k => new KullaniciIpAdresleriListDto
            {
                Id = k.Id,
                KullaniciId = k.KullaniciId,
                KullaniciEPosta = k.Kullanicilar.Eposta,
                KullaniciAdSoyad = k.Kullanicilar.AdSoyad,
                KullaniciTipi = k.Kullanicilar.KullaniciTipi,
                TedarikciId = k.Kullanicilar.TedarikciId,
                TedarikciAdi = k.Kullanicilar.TedarikciId.HasValue ? k.Kullanicilar.Tedarikciler!.Unvan : string.Empty,
                WebsiteId = k.Kullanicilar.WebSiteId,
                WebsiteAdi = k.Kullanicilar.WebSiteId.HasValue ? k.Kullanicilar.WebSiteleri!.WebSiteUrl : string.Empty,
                IpAdresi = k.IpAdresi,
            }
                              )
                             .ToListAsync();
        }

        public async Task<List<KullaniciIpAdresleri>> GetByKullaniciIdAsync(Guid kullaniciId)
        {
            return await _dbContext.Set<KullaniciIpAdresleri>()
                         .Where(k => k.KullaniciId == kullaniciId &&
                                     k.SilindiMi == false)
                         .ToListAsync();

        }
    }
}
