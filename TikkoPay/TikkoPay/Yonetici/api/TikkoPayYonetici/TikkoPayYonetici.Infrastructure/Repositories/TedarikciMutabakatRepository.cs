using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.TedarikciMutabakatlari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
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

        public async Task<IEnumerable<TedarikciMutabakatlariListDto>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi)
        {
            var oncekiGun = mutabakatTarihi.AddDays(-1);

            var tedarikciler = new List<Tedarikciler>();
            var tedarikcilerQuery = _dbSetTedarikciler.AsNoTracking().Where(t => !t.SilindiMi);
            if (tedarikciId.HasValue)
            {
                tedarikcilerQuery = tedarikcilerQuery.Where(k => k.Id == tedarikciId.Value);
            }

            tedarikciler = await tedarikcilerQuery.ToListAsync();

            var tedarikciIds = tedarikciler.Select(t => t.Id).ToList();

            var mutabakatlar = await _dbSet
                .Where(m => m.MutabakatTarihi == mutabakatTarihi && tedarikciIds.Contains(m.TedarikciId))
                .ToListAsync();

            var previousKasalar = await _dbSet
                .Where(m => m.MutabakatTarihi == oncekiGun && tedarikciIds.Contains(m.TedarikciId))
                .ToDictionaryAsync(x => x.TedarikciId, x => x.Kasa);

            var result = tedarikciler
                .Select(t =>
                {
                    var m = mutabakatlar.FirstOrDefault(x => x.TedarikciId == t.Id);

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

        public async Task RemoveFromDb(Guid id)
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TedarikciMutabakatlari WHERE Id = {0}", id);
        }
    }
}
