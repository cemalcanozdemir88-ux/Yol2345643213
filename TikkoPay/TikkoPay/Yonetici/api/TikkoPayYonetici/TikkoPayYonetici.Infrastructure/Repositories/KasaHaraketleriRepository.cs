using TikkoPayYonetici.Core.DTOs.KasaHaraketleri;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class KasaHaraketleriRepository : GenericRepository<KasaHaraketleri>, IKasaHaraketleriRepository
    {
        private readonly DbSet<KasaHaraketleri> _dbSet;

        public KasaHaraketleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<KasaHaraketleri>();
        }

        public async Task<KasaHaraketleriListDto?> GetByIdDtoAsync(Guid id)
        {
            return await _dbSet
               .Include(k => k.Tedarikciler)
               .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
               .OrderByDescending(k => k.KayitTarihi)
               .Select(k => new KasaHaraketleriListDto
               {
                   Id = k.Id,
                   TopId = k.TopId,
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

        private IQueryable<KasaHaraketleri> ApplyFilters(IQueryable<KasaHaraketleri> query, Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            if (tedarikciId is not null)
            {
                query = query.Where(k => k.TedarikciId == tedarikciId);
            }

            if (durum is not null)
            {
                query = query.Where(k => ((int)k.Durum) == durum);
            }

            if (islemTipi is not null)
            {
                query = query.Where(k => ((int)k.IslemTipi) == islemTipi);
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

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                         .Select(k => new KasaHaraketleriListDto
                         {
                             Id = k.Id,
                             TopId = k.TopId,
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

        public async Task<IEnumerable<KasaHaraketleriListDto>> GetAllDtoAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd);

            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new KasaHaraketleriListDto
                         {
                             Id = k.Id,
                             TopId = k.TopId,
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

        public async Task<int> GetCountAsync(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public override Task AddAsync(KasaHaraketleri entity)
        {
            if (!IsSuperAdmin())
            {
                var tedarikciId = base.GetTedarikciId();
                entity.TedarikciId = tedarikciId;
            }

            return base.AddAsync(entity);
        }

        public override void Update(KasaHaraketleri entity, object dto)
        {
            if (!IsSuperAdmin())
            {
                var tedarikciId = base.GetTedarikciId();
                entity.TedarikciId = tedarikciId;
            }

            base.Update(entity, dto);
        }

        public async Task DeleteByTopId(Guid topId)
        {
            var items = await _dbSet.Where(k => k.TopId.Equals(topId)).ToListAsync();
            foreach (var item in items)
            {
                item.SilindiMi = true;
            }
        }
    }
}
