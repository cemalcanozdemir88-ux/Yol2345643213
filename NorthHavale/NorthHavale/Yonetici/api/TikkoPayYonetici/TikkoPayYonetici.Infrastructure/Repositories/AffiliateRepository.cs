using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.Affiliate;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class AffiliateRepository : GenericRepository<Affiliates>, IAffiliateRepository
    {
        private readonly DbSet<Affiliates> _dbSet;
        private readonly DbSet<YatirimCekimTalepleri> _dbSetYatirimTaleperi;

        public AffiliateRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<Affiliates>();
            _dbSetYatirimTaleperi = _dbContext.Set<YatirimCekimTalepleri>();
        }

        public async Task<List<AffiliateDto>> GetAllAsync(Guid? affiliateId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (affiliateId.HasValue)
            {
                query = query.Where(k => k.Id == affiliateId.Value);
            }

            query = query.OrderBy(k => k.Unvan);

            return await query.Select(k => new AffiliateDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                CekimKomisyonOrani = k.CekimKomisyonOrani,
                YaririmKomisyonOrani = k.YaririmKomisyonOrani
            }).ToListAsync();
        }

        public async Task<List<AffiliateBakiyeListDto>> GetBakiyeList(Guid? affiliateId, Guid? websiteId)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (affiliateId.HasValue)
            {
                query = query.Where(k => k.Id == affiliateId.Value);
            }

            var bakiyeList = await query.Select(k => new AffiliateBakiyeListDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                YaririmKomisyonOrani = k.YaririmKomisyonOrani,
                CekimKomisyonOrani = k.CekimKomisyonOrani,
                ToplamYatirim = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteleri.AffiliateId == k.Id &&
                                            (!websiteId.HasValue || y.WebSiteId == websiteId.Value) &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamCekim = _dbSetYatirimTaleperi
                                .Where(y => y.WebSiteleri.AffiliateId == k.Id &&
                                            (!websiteId.HasValue || y.WebSiteId == websiteId.Value) &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
            }).ToListAsync();

            foreach (var item in bakiyeList)
            {
                item.ToplamYatirimKomisyon = item.ToplamYatirim * (decimal)(item.YaririmKomisyonOrani / 100);
                item.ToplamCekimKomisyon = item.ToplamCekim * (decimal)(item.CekimKomisyonOrani / 100);
            }

            return bakiyeList;
        }
    }
}
