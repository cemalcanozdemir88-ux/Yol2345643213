using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class KullanicilarRepository : GenericRepository<Kullanicilar>, IKullanicilarRepository
    {
        public KullanicilarRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public override Task AddAsync(Kullanicilar entity)
        {
            var tedarikciId = base.GetTedarikciId();
            entity.TedarikciId = tedarikciId;

            return base.AddAsync(entity);
        }

        public override void Update(Kullanicilar entity, object dto)
        {
            var tedarikciId = base.GetTedarikciId();
            entity.TedarikciId = tedarikciId;

            base.Update(entity, dto);
        }

        public async Task<KullanicilarListDto> GetByIdDtoAsync(Guid id)
        {
            return await base._dbContext.Set<Kullanicilar>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.WebSiteleri)
                .Where(k => k.Id.Equals(id) && k.KullaniciTipi == KullaniciTip.TedarikciKullanicisi && k.SilindiMi == false)
                .Select(h => new KullanicilarListDto
                {
                    Id = h.Id,
                    TedarikciId = h.TedarikciId,
                    TedarikciAdi = h.Tedarikciler != null ? h.Tedarikciler.Unvan : string.Empty,
                    WebSiteId = h.WebSiteId,
                    WebSiteUrl = h.WebSiteleri != null ? h.WebSiteleri.WebSiteUrl : string.Empty,
                    AdSoyad = h.AdSoyad,
                    Eposta = h.Eposta,
                    KullaniciTipi = h.KullaniciTipi,
                    KullaniciTipiAdi = GetKullaniciTipiAdi(h.KullaniciTipi),
                    IpLoggingEnabled = h.IpLoggingEnabled,
                    Durum = h.Durum,
                    IsOtpEnabled = h.IsOtpEnabled
                })
                .FirstOrDefaultAsync();
        }

        public async Task<KullanicilarListDto> GetByEmailPassword(LoginRequest loginRequest)
        {
            var item = await base._dbContext.Set<Kullanicilar>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.WebSiteleri)
                .Where(k =>
                    k.KullaniciTipi == KullaniciTip.TedarikciKullanicisi &&
                    k.Eposta.Equals(loginRequest.Eposta) &&
                    k.Durum == true &&
                    k.SilindiMi == false)
                .FirstOrDefaultAsync();

            if(item is null)
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
                TedarikciId = item.TedarikciId,
                TedarikciAdi = item.Tedarikciler?.Unvan,
                WebSiteId = item.WebSiteId,
                WebSiteUrl = item.WebSiteleri?.WebSiteUrl,
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
                        .Include(k => k.Tedarikciler)
                        .Include(k => k.WebSiteleri)
                        .Where(k => k.SilindiMi == false);

            var tedarikciId = base.GetTedarikciId();
            query = query.Where(k => k.TedarikciId == tedarikciId);

            if (kullaniciTipi.HasValue)
            {
                query = query.Where(k => k.KullaniciTipi == kullaniciTipi);
            }

            return await query.OrderBy(k => k.TedarikciId).ThenByDescending(k => k.KayitTarihi)
                              .Select(h => new KullanicilarListDto
                                {
                                    Id = h.Id,
                                    TedarikciId = h.TedarikciId,
                                    TedarikciAdi = h.Tedarikciler != null ? h.Tedarikciler.Unvan : string.Empty,
                                    WebSiteId = h.WebSiteId,
                                    WebSiteUrl = h.WebSiteleri != null ? h.WebSiteleri.WebSiteUrl : string.Empty,
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
                674 => "Tedarikci Kullanıcısı",
                951 => "Web Site Kullanıcısı",
                _ => ""
            };
        }
    }
}
