using PaparaYonetici.Core.DTOs.WebSiteleri;
using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace PaparaYonetici.Infrastructure.Repositories
{
    public class WebSiteleriRepository : GenericRepository<WebSiteleri>, IWebSiteleriRepository
    {
        private readonly DbSet<WebSiteleri> _dbSet;
        private readonly DbSet<YatirimCekimTalepleri> _dbSetYatirimTaleperi;
        private readonly DbSet<WebSiteKasaHaraketleri> _dbSetWebSiteKasaHaraketleri;

        public WebSiteleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<WebSiteleri>();
            _dbSetYatirimTaleperi = _dbContext.Set<YatirimCekimTalepleri>();
            _dbSetWebSiteKasaHaraketleri = _dbContext.Set<WebSiteKasaHaraketleri>();
        }

        public async Task<List<WebSiteleriSelectListDto>> GetSelectListAsync(Guid? webSiteId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (webSiteId.HasValue)
            {
                query = query.Where(k => k.Id == webSiteId.Value);
            }

            return await query.Select(k => new WebSiteleriSelectListDto
            {
                Id = k.Id,
                WebSiteUrl = k.WebSiteUrl
            }).ToListAsync();
        }

        public async Task<List<WebSiteleriSelectListDto>> GetSelectLisByAffiliateAsync(Guid affiliateId)
        {
            var query = _dbSet.Where(k => k.AffiliateId == affiliateId && k.SilindiMi == false);
            return await query.Select(k => new WebSiteleriSelectListDto
            {
                Id = k.Id,
                WebSiteUrl = k.WebSiteUrl
            }).ToListAsync();
        }

        public async Task<List<WebSiteleriListDto>> GetAllAsync(Guid? webSiteId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (webSiteId.HasValue)
            {
                query = query.Where(k => k.Id == webSiteId.Value);
            }

            return await query.Select(k => new WebSiteleriListDto
            {
                Id = k.Id,
                WebSiteUrl = k.WebSiteUrl,
                YaririmKomisyonOrani = k.YaririmKomisyonOrani,
                CekimKomisyonOrani = k.CekimKomisyonOrani,
                OdemeKomisyonOrani = k.OdemeKomisyonOrani,
                AffiliateId = k.AffiliateId,
                AffiliateUnvan = k.AffiliateId.HasValue ? k.Affiliate.Unvan : string.Empty,
                Type = k.Type
            }).ToListAsync();
        }

        public async Task<List<WebSiteBakiyeListDto>> GetBakiyeList(Guid? webSiteId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (webSiteId.HasValue)
            {
                query = query.Where(k => k.Id == webSiteId.Value);
            }

            return await query.Select(k => new WebSiteBakiyeListDto
            {
                Id = k.Id,
                WebSiteUrl = k.WebSiteUrl,
                ToplamYatirim = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamYatirimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.WebSiteKomisyon),
                ToplamCekim = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamCekimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                            y.SilindiMi == false)
                                .Sum(k => k.WebSiteKomisyon),
                ToplamKasaHaraketiGiris = _dbSetWebSiteKasaHaraketleri
                                          .Where(y => y.WebSitesiId == k.Id &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                 y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris &&
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar),
                ToplamKasaHaraketiCikis = _dbSetWebSiteKasaHaraketleri
                                          .Where(y => y.WebSitesiId == k.Id &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                 y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis &&
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar)

            }).ToListAsync();
        }
    }
}
