using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParaYolYonetici.Core.DTOs.WebSiteMutabakatlari;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public class WebSiteMutabakatRepository : GenericRepository<WebSiteMutabakatlari>, IWebSiteMutabakatRepository
    {
        private DbSet<WebSiteMutabakatlari> _dbSet;
        private DbSet<WebSiteleri> _dbSetWebSiteler;

        public WebSiteMutabakatRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<WebSiteMutabakatlari>();
            _dbSetWebSiteler = _dbContext.Set<WebSiteleri>();
        }

        public async Task<IEnumerable<WebSiteMutabakatlariListDto>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi)
        {
            var oncekiGun = mutabakatTarihi.AddDays(-1);

            var webSiteleri = new List<WebSiteleri>();
            var webSiteleriQuery = _dbSetWebSiteler.AsNoTracking().Where(w => !w.SilindiMi);
            if (webSiteId.HasValue)
            {
                webSiteleriQuery = webSiteleriQuery.Where(w => w.Id == webSiteId.Value);
            }

            webSiteleri = await webSiteleriQuery.ToListAsync();

            var webSiteleriIds = webSiteleri.Select(w => w.Id).ToList();

            var mutabakatlar = await _dbSet
                .Where(m => m.MutabakatTarihi == mutabakatTarihi && webSiteleriIds.Contains(m.WebSiteId))
                .ToListAsync();

            var previousKasalar = await _dbSet
                .Where(m => m.MutabakatTarihi == oncekiGun && webSiteleriIds.Contains(m.WebSiteId))
                .ToDictionaryAsync(x => x.WebSiteId, x => x.Kasa);

            var result = webSiteleri
                .Select(t =>
                {
                    var m = mutabakatlar.FirstOrDefault(x => x.WebSiteId == t.Id);

                    decimal devir = (m != null && m.Devir != 0)
                        ? m.Devir
                        : (previousKasalar.TryGetValue(t.Id, out var kasa) ? kasa : 0);

                    return new WebSiteMutabakatlariListDto
                    {
                        Id = m?.Id ?? Guid.Empty,
                        WebSiteId = t.Id,
                        WebSiteAdi = t.Baslik!,

                        Devir = devir,
                        ToplamYatirim = m?.ToplamYatirim ?? 0,
                        ToplamYatirimKomisyon = m?.ToplamYatirimKomisyon ?? 0,
                        ToplamCekim = m?.ToplamCekim ?? 0,
                        ToplamCekimKomisyon = m?.ToplamCekimKomisyon ?? 0,
                        ToplamGiris = m?.ToplamGiris ?? 0,
                        ToplamCikis = m?.ToplamCikis ?? 0,
                        ToplamCikisKomisyon = m?.ToplamCikisKomisyon ?? 0,
                        ToplamGirisManual = m?.ToplamGirisManual ?? 0,
                        ToplamCikisManual = m?.ToplamCikisManual ?? 0,
                        ToplamVirmanGiris = m?.ToplamVirmanGiris ?? 0,
                        ToplamVirmanCikis = m?.ToplamVirmanCikis ?? 0,
                        Kasa = m?.Kasa ?? 0,
                        MutabakatTarihi = (m?.MutabakatTarihi ?? mutabakatTarihi).ToString("yyyy-MM-dd")
                    };
                })
                .OrderBy(x => x.WebSiteAdi)
                .ToList();

            return result;
        }

        public async Task RemoveFromDb(Guid id)
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM WebSiteMutabakatlari WHERE Id = {0}", id);
        }
    }
}
