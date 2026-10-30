using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayFixYonetici.Core.DTOs.KasaHaraketleri;
using PayFixYonetici.Core.Interfaces;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Repositories
{
    public class KasaHaraketleriRepository : GenericRepository<KasaHaraketleri>, IKasaHaraketleriRepository
    {
        private readonly DbSet<KasaHaraketleri> _dbSet;
        private readonly DbSet<YatirimTalepleri> _dbSetYatirimTalepleri;
        private readonly DbSet<CekimTalepleri> _dbSetCekimTalepleri;
        private readonly DbSet<KasaHaraketleri> _dbSetKasaHaraketleri;

        public KasaHaraketleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<KasaHaraketleri>();
            _dbSetYatirimTalepleri = _dbContext.Set<YatirimTalepleri>();
            _dbSetCekimTalepleri = _dbContext.Set<CekimTalepleri>();
            _dbSetKasaHaraketleri = _dbContext.Set<KasaHaraketleri>();
        }

        public async Task<KasaHaraketleriListDto?> GetByIdDtoAsync(Guid id)
        {
            return await _dbSet
               .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
               .OrderByDescending(k => k.KayitTarihi)
               .Select(k => new KasaHaraketleriListDto
               {
                   Id = k.Id,
                   Aciklama = k.Aciklama,
                   Tutar = k.Tutar,
                   DovizKuru = k.DovizKuru,
                   UsdtMiktar = k.UsdtMiktar,
                   UsdtTxtNo = k.UsdtTxtNo,
                   UsdtAdres = k.UsdtAdres,
                   ResimUrl = k.ResimUrl,
                   Durum = k.Durum,
                   IslemTipi = k.IslemTipi,
                   KayitTarihi = k.KayitTarihi,
                   KaydedenKullanici = k.KaydedenKullanici.AdSoyad,
                   GuncellemeTarihi = k.GuncellemeTarihi,
                   GuncelleyenKullanici = k.GuncelleyenKullanici.AdSoyad
               })
               .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, durum, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                         .Select(k => new KasaHaraketleriListDto
                         {
                             Id = k.Id,
                             Aciklama = k.Aciklama,
                             Tutar = k.Tutar,
                             DovizKuru = k.DovizKuru,
                             UsdtMiktar = k.UsdtMiktar,
                             UsdtTxtNo = k.UsdtTxtNo,
                             UsdtAdres = k.UsdtAdres,
                             ResimUrl = k.ResimUrl,
                             Durum = k.Durum,
                             IslemTipi = k.IslemTipi,
                             KayitTarihi = k.KayitTarihi,
                             KaydedenKullanici = k.KaydedenKullanici.AdSoyad,
                             GuncellemeTarihi = k.GuncellemeTarihi,
                             GuncelleyenKullanici = k.GuncelleyenKullanici.AdSoyad
                         })
                         .ToListAsync();
        }

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbSet
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, durum, createdDateStart, createdDateEnd);

            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new KasaHaraketleriListDto
            {
                Id = k.Id,
                Aciklama = k.Aciklama,
                Tutar = k.Tutar,
                DovizKuru = k.DovizKuru,
                UsdtMiktar = k.UsdtMiktar,
                UsdtTxtNo = k.UsdtTxtNo,
                UsdtAdres = k.UsdtAdres,
                ResimUrl = k.ResimUrl,
                Durum = k.Durum,
                IslemTipi = k.IslemTipi,
                KayitTarihi = k.KayitTarihi,
                KaydedenKullanici = k.KaydedenKullanici.AdSoyad,
                GuncellemeTarihi = k.GuncellemeTarihi,
                GuncelleyenKullanici = k.GuncelleyenKullanici.AdSoyad
            }).ToListAsync();
        }

        public async Task<int> GetCountAsync(int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, durum, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public override Task AddAsync(KasaHaraketleri entity)
        {
            return base.AddAsync(entity);
        }

        public override void Update(KasaHaraketleri entity, object dto)
        {
            base.Update(entity, dto);
        }

        public List<BakiyeListDto> GetBakiyeList()
        {
            var yatirimToplamlar = _dbSetYatirimTalepleri.AsNoTracking()
                .Where(y => !y.SilindiMi)
                .GroupBy(_ => 1) 
                .Select(g => new
                {
                    ToplamYatirim = g.Sum(k => k.Tutar),
                    ToplamYatirimKomisyon = g.Sum(k => k.WebSiteKomisyon)
                })
                .FirstOrDefault();

            var cekimToplamlar = _dbSetCekimTalepleri.AsNoTracking()
                .Where(y => !y.SilindiMi && y.Durum)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    ToplamCekim = g.Sum(k => k.Tutar),
                    ToplamCekimKomisyon = g.Sum(k => k.WebSiteKomisyon)
                })
                .FirstOrDefault();

            var kasaToplamlar = _dbSetKasaHaraketleri.AsNoTracking()
                .Where(y => y.Durum == Core.Enums.Durum.Onaylandi && !y.SilindiMi)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    ToplamGiris = g.Where(k => k.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris)
                                   .Sum(k => k.Tutar),
                    ToplamCikis = g.Where(k => k.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis)
                                   .Sum(k => k.Tutar)
                })
                .FirstOrDefault();

            
            var item = new BakiyeListDto
            {
                Id = Guid.NewGuid(),
                Unvan = "PayFix",
                ToplamYatirim = yatirimToplamlar?.ToplamYatirim ?? 0,
                ToplamYatirimKomisyon = yatirimToplamlar?.ToplamYatirimKomisyon ?? 0,
                ToplamCekim = cekimToplamlar?.ToplamCekim ?? 0,
                ToplamCekimKomisyon = cekimToplamlar?.ToplamCekimKomisyon ?? 0,
                ToplamKasaHaraketiGiris = kasaToplamlar?.ToplamGiris ?? 0,
                ToplamKasaHaraketiCikis = kasaToplamlar?.ToplamCikis ?? 0
            };

            return new List<BakiyeListDto> { item };
        }

        private IQueryable<KasaHaraketleri> ApplyFilters(IQueryable<KasaHaraketleri> query, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            if (durum is not null)
            {
                query = query.Where(k => ((int)k.Durum) == durum);
            }

            if (createdDateStart is not null)
            {
                createdDateStart = new DateTime(createdDateStart.Value.Year, createdDateStart.Value.Month, createdDateStart.Value.Day, 0, 0, 0);
                query = query.Where(k => k.KayitTarihi >= createdDateStart);
            }

            if (createdDateEnd is not null)
            {
                createdDateEnd = new DateTime(createdDateEnd.Value.Year, createdDateEnd.Value.Month, createdDateEnd.Value.Day, 0, 0, 0).AddDays(1);
                query = query.Where(k => k.KayitTarihi <= createdDateEnd);
            }

            return query;
        }

    }
}
