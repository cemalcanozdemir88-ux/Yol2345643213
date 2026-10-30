using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Infrastructure.Repositories
{
    public class YatirimTalepleriRepository : GenericRepository<YatirimCekimTalepleri>, IYatirimTalepleriRepository
    {
        public YatirimTalepleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<YatirimTalepleriListDto?> GetByIdDtoAsync(Guid id)
        {
            return await _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                             .Select(k => new YatirimTalepleriListDto
                             {
                                 Id = k.Id,
                                 TedarikciId = k.TedarikciId,
                                 TedarikciAdi = k.Tedarikciler.Unvan,
                                 BankaId = k.BankaId,
                                 BankaAdi = k.Bankalar.Baslik,
                                 WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                                 WebSiteTransactionId = k.WebSiteTransactionId,
                                 AdSoyad = k.AdSoyad,
                                 YontemTipi = k.YontemTipi,
                                 HesapSahibi = k.HesapSahibi,
                                 HesapNo = k.HesapNo,
                                 Tutar = k.Tutar,
                                 TedarikciKomisyon = k.TedarikciKomisyon,
                                 Toplam = k.Toplam,
                                 IslemTipi = k.IslemTipi,
                                 IptalAciklama = k.IptalAciklama,
                                 Durum = k.Durum,
                                 OnayTarihi = k.OnayTarihi,
                                 KayitTarihi = k.KayitTarihi
                             }).FirstOrDefaultAsync();
        }

        private IQueryable<YatirimCekimTalepleri> ApplyFilters(IQueryable<YatirimCekimTalepleri> query, 
            Guid? tedarikciId, 
            Guid? bankaId, 
            Guid? webSiteId,
            string searchText, 
            DateTime? approveDateStart,
            DateTime? approveDateEnd,
            DateTime? createdDateStart, 
            DateTime? createdDateEnd)
        {
            if (tedarikciId.HasValue)
            {
                query = query.Where(k => k.TedarikciId == tedarikciId);
            }

            if (bankaId.HasValue)
            {
                query = query.Where(k => k.BankaId == bankaId);
            }

            if (webSiteId.HasValue)
            {
                query = query.Where(k => k.WebSiteId == webSiteId);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => (k.TalepNo + k.IdNumber.ToString()).Contains(searchText) ||
                                         k.WebSiteTransactionId.Contains(searchText) ||
                                         k.AdSoyad.Contains(searchText) ||
                                         k.YontemTipi.Contains(searchText) ||
                                         k.HesapSahibi.Contains(searchText) ||
                                         k.HesapNo.Contains(searchText) ||
                                         k.UserName!.Contains(searchText) ||
                                         k.UserId!.Contains(searchText));
            }

            if(approveDateStart is not null)
            {
                approveDateStart = new DateTime(approveDateStart.Value.Year, approveDateStart.Value.Month, approveDateStart.Value.Day, 0, 0, 0);
                query = query.Where(k => k.OnayTarihi >= approveDateStart);
            }

            if (approveDateEnd is not null)
            {
                approveDateEnd = new DateTime(approveDateEnd.Value.Year, approveDateEnd.Value.Month, approveDateEnd.Value.Day, 0, 0, 0).AddDays(1);
                query = query.Where(k => k.OnayTarihi <= approveDateEnd);
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

        public async Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query =  _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                             .Select(k => new YatirimTalepleriListDto
                             {
                                 Id = k.Id,
                                 TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                                 TedarikciId = k.TedarikciId,
                                 TedarikciAdi = k.Tedarikciler.Unvan,
                                 BankaId = k.BankaId,
                                 BankaAdi = k.Bankalar.Baslik,
                                 WebSiteUrl = showWebSite ? k.WebSiteleri.WebSiteUrl : string.Empty,
                                 WebSiteTransactionId = showWebSite ? k.WebSiteTransactionId : string.Empty,
                                 AdSoyad = k.AdSoyad,
                                 UserName = k.UserName,
                                 UserId = k.UserId,
                                 YontemTipi = k.YontemTipi  ,
                                 HesapSahibi = (islemTipi == IslemTip.Withdraw && durum == Durum.Bekliyor) ? string.Empty : k.HesapSahibi,
                                 HesapNo = k.HesapNo,
                                 Tutar = k.Tutar,
                                 TedarikciKomisyon = k.TedarikciKomisyon,
                                 Toplam = k.Toplam,
                                 IslemTipi = k.IslemTipi,
                                 IptalAciklama = k.IptalAciklama,
                                 Durum = k.Durum,
                                 OnayTarihi = k.OnayTarihi,
                                 GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                                 GuncellemeTarihi = k.GuncellemeTarihi,
                                 KayitTarihi = k.KayitTarihi
                             })
                             .ToListAsync();

        }

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriListDto
            {
                Id = k.Id,
                TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                TedarikciId = k.TedarikciId,
                TedarikciAdi = k.Tedarikciler.Unvan,
                BankaId = k.BankaId,
                BankaAdi = k.Bankalar.Baslik,
                WebSiteUrl = showWebSite ? k.WebSiteleri.WebSiteUrl : string.Empty,
                WebSiteTransactionId = showWebSite ? k.WebSiteTransactionId : string.Empty,
                AdSoyad = k.AdSoyad,
                UserName = k.UserName,
                UserId = k.UserId,
                YontemTipi = k.YontemTipi,
                HesapSahibi = (islemTipi == IslemTip.Withdraw && durum == Durum.Bekliyor) ? string.Empty : k.HesapSahibi,
                HesapNo = k.HesapNo,
                Tutar = k.Tutar,
                TedarikciKomisyon = k.TedarikciKomisyon,
                Toplam = k.Toplam,
                IslemTipi = k.IslemTipi,
                IptalAciklama = k.IptalAciklama,
                Durum = k.Durum,
                OnayTarihi = k.OnayTarihi,
                GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                GuncellemeTarihi = k.GuncellemeTarihi,
                YonlendirenKullanici = k.YonlendirenKullaniciId.HasValue ? k.YonlendirenKullanici.AdSoyad : string.Empty,
                YonlendirilmeTarihi = k.YonlendirilmeTarihi,
                KayitTarihi = k.KayitTarihi
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<YatirimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriBaseListDto
            {
                Id = k.Id,
                TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                WebSiteTransactionId = k.WebSiteTransactionId,
                AdSoyad = k.AdSoyad,
                UserName = k.UserName,
                UserId = k.UserId,
                Tutar = k.Tutar,
                OnayTarihi = k.OnayTarihi,
                KayitTarihi = k.KayitTarihi
            })
            .ToListAsync();
        }


        public override Task AddAsync(YatirimCekimTalepleri entity)
        {
            entity.TalepNo = DateTime.Now.ToString("yyyyMMdd");

            return base.AddAsync(entity);
        }

        public async Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == IslemTip.Deposit &&
                                         k.Durum == Durum.Onaylandi &&
                                         !k.SilindiMi);
            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var result = await query.ToListAsync();

            return new ToplamYatirimTutariDto
            {
                ToplamTutar = result.Sum(k => k.Tutar),
                ToplamKomisyon = result.Sum(k => k.TedarikciKomisyon)
            };
        }

        public async Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == IslemTip.Withdraw &&
                                         k.Durum == Durum.Onaylandi &&
                                         !k.SilindiMi);
            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var result = await query.ToListAsync();

            return new ToplamCekimTutariDto
            {
                ToplamTutar = result.Sum(k => k.Tutar),
                ToplamKomisyon = result.Sum(k => k.TedarikciKomisyon)
            };
        }
    }
}
