using TedarikciHizmet.Core.DTOs.WebSiteKasaHaraketleri;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class WebSiteKasaHaraketleriRepository : GenericRepository<WebSiteKasaHaraketleri>, IWebSiteKasaHaraketleriRepository
    {
        private readonly DbSet<WebSiteKasaHaraketleri> _dbSet;

        public WebSiteKasaHaraketleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<WebSiteKasaHaraketleri>();
        }

        public async Task<WebSiteKasaHaraketleriListDto> GetByIdDtoAsync(Guid id)
        {
            return await _dbSet
               .Include(k => k.WebSiteleri)
               .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
               .OrderByDescending(k => k.KayitTarihi)
               .Select(k => new WebSiteKasaHaraketleriListDto
               {
                   Id = k.Id,
                   WebSitesiId = k.WebSitesiId,
                   WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                   Aciklama = k.Aciklama,
                   Tutar = k.Tutar,
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

        public async Task<IEnumerable<WebSiteKasaHaraketleriListDto>> GetAllDtoAsync(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbSet
                        .Include(k => k.WebSiteleri)
                        .Include(k => k.KaydedenKullanici)
                        .Include(k => k.GuncelleyenKullanici)
                        .Where(k => k.SilindiMi == false);

            if (webSiteId is not null)
            {
                query = query.Where(k => k.WebSitesiId == webSiteId);
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

            return await query.OrderBy(k => k.WebSitesiId).ThenByDescending(k => k.KayitTarihi)
                         .Select(k => new WebSiteKasaHaraketleriListDto
                         {
                             Id = k.Id,
                             WebSitesiId = k.WebSitesiId,
                             WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                             Aciklama = k.Aciklama,
                             Tutar = k.Tutar,
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
    }
}
