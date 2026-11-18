using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TedarikciHizmet.Core.DTOs.TedarikciMutabakatlari;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class TedarikciMutabakatRepository : GenericRepository<TedarikciMutabakatlari>, ITedarikciMutabakatRepository
    {
        private readonly DbSet<TedarikciMutabakatlari> _dbSet;
        private readonly DbSet<Tedarikciler> _dbSetTedarikciler;

        public TedarikciMutabakatRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<TedarikciMutabakatlari>();
            _dbSetTedarikciler = _dbContext.Set<Tedarikciler>();
        }

        public async Task<IEnumerable<TedarikciMutabakatlariListDto>> GetAllDtoAsync(Guid tedarikciId, DateOnly mutabakatTarihi)
        {
            var oncekiGun = mutabakatTarihi.AddDays(-1);

            var tedarikciler = new List<Tedarikciler>();
            var tedarikcilerQuery = _dbSetTedarikciler.AsNoTracking().Where(t => !t.SilindiMi);
            tedarikcilerQuery = tedarikcilerQuery.Where(k => k.Id == tedarikciId);

            tedarikciler = await tedarikcilerQuery.ToListAsync();

            var tedarikciIds = tedarikciler.Select(t => t.Id).ToList();

            var mutabakatlar = await _dbSet
                .Where(m => m.MutabakatTarihi == mutabakatTarihi && tedarikciIds.Contains(m.TedarikciId))
                .ToListAsync();

            var previousMutabakatlar = await _dbSet
                .Where(m => m.MutabakatTarihi == oncekiGun && tedarikciIds.Contains(m.TedarikciId))
                .ToListAsync();
            // Aynı güne birden fazla kayıt varsa en son kaydedileni kullan (çift kayıt sayfayı/hesabı bozmasın)
            var previousKasalar = previousMutabakatlar
                .GroupBy(x => x.TedarikciId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.KayitTarihi).First().Kasa);

            var result = tedarikciler
                .Select(t =>
                {
                    var m = mutabakatlar.Where(x => x.TedarikciId == t.Id).OrderByDescending(x => x.KayitTarihi).FirstOrDefault();

                    decimal devir = (m != null && m.Devir != 0)
                        ? m.Devir
                        : (previousKasalar.TryGetValue(t.Id, out var kasa) ? kasa : 0);

                    return new TedarikciMutabakatlariListDto
                    {
                        Id = m?.Id ?? Guid.Empty,
                        TedarikciId = t.Id,
                        TedarikciAdi = t.Unvan,

                        Devir = devir,
                        ToplamYatirim = m?.ToplamYatirim ?? 0,
                        ToplamYatirimKomisyon = m?.ToplamYatirimKomisyon ?? 0,
                        ToplamCekim = m?.ToplamCekim ?? 0,
                        ToplamCekimKomisyon = m?.ToplamCekimKomisyon ?? 0,
                        ToplamGiris = m?.ToplamGiris ?? 0,
                        ToplamGirisKomisyon = m?.ToplamGirisKomisyon ?? 0,
                        ToplamCikis = m?.ToplamCikis ?? 0,
                        ToplamGirisManual = m?.ToplamGirisManual ?? 0,
                        ToplamCikisManual = m?.ToplamCikisManual ?? 0,
                        ToplamVirmanGiris = m?.ToplamVirmanGiris ?? 0,
                        ToplamVirmanCikis = m?.ToplamVirmanCikis ?? 0,
                        Kasa = m?.Kasa ?? 0,
                        MutabakatTarihi = (m?.MutabakatTarihi ?? mutabakatTarihi).ToString("yyyy-MM-dd")
                    };
                })
                .OrderBy(x => x.TedarikciAdi)
                .ToList();

            return result;
        }
    }
}
