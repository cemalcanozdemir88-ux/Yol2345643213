using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayCoYonetici.Core.DTOs.Kullanicilar;
using PayCoYonetici.Core.Enums;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Infrastructure.Repositories
{
    public class KullanicilarRepository : GenericRepository<Kullanicilar>, IKullanicilarRepository
    {
        public KullanicilarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public override Task AddAsync(Kullanicilar entity)
        {
            return base.AddAsync(entity);
        }

        public override void Update(Kullanicilar entity, object dto)
        {
            base.Update(entity, dto);
        }

        public async Task<KullanicilarListDto?> GetByIdDtoAsync(Guid id)
        {
            return await base._dbContext.Set<Kullanicilar>()
                .Include(k => k.WebSiteleri)
                .Include(k => k.Affiliates)
                .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                .Select(h => new KullanicilarListDto
                {
                    Id = h.Id,
                    WebSiteId = h.WebSiteId,
                    WebSiteUrl = h.WebSiteleri != null ? h.WebSiteleri.WebSiteUrl : string.Empty,
                    AffiliateId = h.AffiliateId,
                    AffiliateUnvan = h.Affiliates != null ? h.Affiliates.Unvan : string.Empty,
                    AdSoyad = h.AdSoyad,
                    Eposta = h.Eposta,
                    KullaniciTipi = h.KullaniciTipi,
                    KullaniciTipiAdi = GetKullaniciTipiAdi(h.KullaniciTipi),
                    Durum = h.Durum,
                    IsOtpEnabled = h.IsOtpEnabled
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUniqueEmail(string email)
        {
            var item = await base._dbContext.Set<Kullanicilar>().Where(k => k.Eposta.Equals(email) && !k.SilindiMi).CountAsync();
            return item == 0;
        }

        public async Task<KullanicilarListDto?> GetByEmailPassword(LoginRequest loginRequest)
        {
            var item = await base._dbContext.Set<Kullanicilar>()
                .Include(k => k.WebSiteleri)
                .Include(k => k.Affiliates)
                .Where(k =>
                    k.Eposta.Equals(loginRequest.Eposta) &&
                   (k.KullaniciTipi == KullaniciTip.Admin || k.KullaniciTipi == KullaniciTip.WebSiteKullanicisi || k.KullaniciTipi == KullaniciTip.AffiliateKullanicisi) &&
                    k.SilindiMi == false)
                .FirstOrDefaultAsync();

            if (item is null)
            {
                return null;
            }

            if (!item.VerifyPassword(loginRequest.Sifre))
            {
                return null;
            }

            return new KullanicilarListDto
            {
                Id = item.Id,
                WebSiteId = item.WebSiteId,
                WebSiteUrl = item.WebSiteleri?.WebSiteUrl,
                AffiliateId = item.AffiliateId,
                AffiliateUnvan = item.Affiliates?.Unvan,
                AdSoyad = item.AdSoyad,
                Eposta = item.Eposta,
                KullaniciTipi = item.KullaniciTipi,
                KullaniciTipiAdi = GetKullaniciTipiAdi(item.KullaniciTipi),
                Durum = item.Durum,
                IsOtpEnabled = item.IsOtpEnabled
            };
        }

        public async Task<IEnumerable<KullanicilarListDto>> GetAllDtoAsync(KullaniciTip? kullaniciTipi)
        {
            var query = base._dbContext.Set<Kullanicilar>()
                        .Include(k => k.WebSiteleri)
                        .Include(k => k.Affiliates)
                        .Where(k => k.SilindiMi == false);

            if (kullaniciTipi.HasValue)
            {
                query = query.Where(k => k.KullaniciTipi == kullaniciTipi);
            }

            return await query.OrderByDescending(k => k.KayitTarihi)
                              .Select(h => new KullanicilarListDto
                              {
                                  Id = h.Id,
                                  WebSiteId = h.WebSiteId,
                                  WebSiteUrl = h.WebSiteleri != null ? h.WebSiteleri.WebSiteUrl : string.Empty,
                                  AffiliateId = h.AffiliateId,
                                  AffiliateUnvan = h.Affiliates != null ? h.Affiliates.Unvan : string.Empty,
                                  AdSoyad = h.AdSoyad,
                                  Eposta = h.Eposta,
                                  KullaniciTipi = h.KullaniciTipi,
                                  KullaniciTipiAdi = GetKullaniciTipiAdi(h.KullaniciTipi),
                                  Durum = h.Durum,
                                  IsOtpEnabled = h.IsOtpEnabled
                              })
                             .ToListAsync();
        }

        protected static string GetKullaniciTipiAdi(KullaniciTip kullaniciTipi)
        {
            return ((int)kullaniciTipi) switch
            {
                759 => "Site Yöneticisi",
                951 => "Web Site Kullanıcısı",
                593 => "Affiliate Kullanıcısı",
                _ => ""
            };
        }
    }
}
