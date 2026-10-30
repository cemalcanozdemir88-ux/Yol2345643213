using TedarikciHizmet.Core.DTOs.KasaHaraketleri;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class KasaHaraketleriRepository : GenericRepository<KasaHaraketleri>, IKasaHaraketleriRepository
    {
        private readonly DbSet<KasaHaraketleri> _dbSet;

        public KasaHaraketleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<KasaHaraketleri>();
        }

        public async Task<KasaHaraketleriListDto> GetByIdDtoAsync(Guid id)
        {
            return await _dbSet
               .Include(k => k.Tedarikciler)
               .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
               .OrderByDescending(k => k.KayitTarihi)
               .Select(k => new KasaHaraketleriListDto
               {
                   Id = k.Id,
                   TedarikciId = k.TedarikciId,
                   TedarikciAdi = k.Tedarikciler.Unvan,
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

        private static IQueryable<KasaHaraketleri> ApplyFilters(IQueryable<KasaHaraketleri> query, Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            if (tedarikciId is not null)
            {
                query = query.Where(k => k.TedarikciId == tedarikciId);
            }

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

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                         .Select(k => new KasaHaraketleriListDto
                         {
                             Id = k.Id,
                             TedarikciId = k.TedarikciId,
                             TedarikciAdi = k.Tedarikciler.Unvan,
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

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, createdDateStart, createdDateEnd);

            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new KasaHaraketleriListDto
                         {
                             Id = k.Id,
                             TedarikciId = k.TedarikciId,
                             TedarikciAdi = k.Tedarikciler.Unvan,
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

        public async Task<int> GetCountAsync(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public override Task AddAsync(KasaHaraketleri entity)
        {
            var tedarikciId = base.GetTedarikciId();
            entity.TedarikciId = tedarikciId;

            return base.AddAsync(entity);
        }

        public override void Update(KasaHaraketleri entity, object dto)
        {
            var tedarikciId = base.GetTedarikciId();
            entity.TedarikciId = tedarikciId;

            base.Update(entity, dto);
        }

        public async Task<decimal> GetToplamKasaHaraketiGiris(Guid tedarikciId)
        {
            var toplamKasaHaraketiGiris = await _dbSet
                                          .Where(y => y.TedarikciId == tedarikciId &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                 (y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.GirisManuel || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanCikis) &&
                                                 y.SilindiMi == false)
                                          .SumAsync(k => k.Tutar);

            return toplamKasaHaraketiGiris;
        }

        public async Task<decimal> GetToplamKasaHaraketiCikis(Guid tedarikciId)
        {
            var toplamKasaHaraketiCikis = await _dbSet
                                          .Where(y => y.TedarikciId == tedarikciId &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                   (y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.CikisManuel || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanGiris) &&
                                                 y.SilindiMi == false)
                                          .SumAsync(k => k.Tutar);

            return toplamKasaHaraketiCikis;
        }
    }
}
