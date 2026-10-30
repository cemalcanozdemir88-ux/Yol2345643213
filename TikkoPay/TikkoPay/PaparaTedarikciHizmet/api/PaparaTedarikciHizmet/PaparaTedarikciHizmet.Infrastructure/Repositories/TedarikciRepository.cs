using PaparaTedarikciHizmet.Core.DTOs.Tedarikciler;
using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace PaparaTedarikciHizmet.Infrastructure.Repositories
{
    public class TedarikciRepository : GenericRepository<Tedarikciler>, ITedarikciRepository
    {
        private readonly DbSet<Tedarikciler> _dbSet;
        private readonly DbSet<YatirimCekimTalepleri> _dbSetYatirimTaleperi;
        private readonly DbSet<KasaHaraketleri> _dbSetKasaHaraketleri;

        public TedarikciRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<Tedarikciler>();
            _dbSetYatirimTaleperi = _dbContext.Set<YatirimCekimTalepleri>();
            _dbSetKasaHaraketleri = _dbContext.Set<KasaHaraketleri>();
        }

        public async Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (tedarikciId.HasValue)
            {
                query = query.Where(k => k.Id == tedarikciId.Value);
            }

            return await query.Select(k => new TedarikciBakiyeListDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                ToplamYatirim = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id && 
                                            y.Durum == Core.Enums.Durum.Onaylandi && 
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit && 
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamYatirimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.TedarikciKomisyon),
                ToplamCekim = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdrawal &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamCekimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdrawal &&
                                            y.SilindiMi == false)
                                .Sum(k => k.TedarikciKomisyon),
                ToplamKasaHaraketiGiris = _dbSetKasaHaraketleri
                                          .Where(y => y.TedarikciId == k.Id && 
                                                 y.Durum == Core.Enums.Durum.Onaylandi && 
                                                 y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris && 
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar),
                ToplamKasaHaraketiCikis = _dbSetKasaHaraketleri
                                          .Where(y => y.TedarikciId == k.Id &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                 y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis &&
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar)

            }).ToListAsync();
        }
    }
}
