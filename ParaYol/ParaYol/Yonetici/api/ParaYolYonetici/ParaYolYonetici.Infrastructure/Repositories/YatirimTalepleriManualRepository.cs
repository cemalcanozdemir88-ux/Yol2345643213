using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParaYolYonetici.Core.DTOs.YatirimTalepleriManual;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public class YatirimTalepleriManualRepository : GenericRepository<YatirimCekimTalepleriManual>, IYatirimTalepleriManualRepository
    {
        public YatirimTalepleriManualRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<YatirimTalepleriManualListDto> GetByIdWitnRelations(Guid id)
        {
            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Where(k => k.Id == id && k.SilindiMi == false)
                             .Select(k => new YatirimTalepleriManualListDto
                             {
                                 Id = k.Id,
                                 TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                                 TedarikciId = k.TedarikciId,
                                 TedarikciAdi = k.Tedarikciler.Unvan,
                                 BankaId = k.BankaId,
                                 BankaAdi = k.Bankalar.Baslik,
                                 AdSoyad = k.AdSoyad,
                                 YontemTipi = k.YontemTipi,
                                 HesapSahibi = k.HesapSahibi,
                                 HesapNo = k.HesapNo,
                                 HesapNoBanka = k.HesapNoBanka,
                                 Tutar = k.Tutar,
                                 TedarikciKomisyon = k.TedarikciKomisyon,
                                 Toplam = k.Toplam,
                                 IslemTipi = k.IslemTipi,
                                 IptalAciklama = k.IptalAciklama,
                                 Durum = k.Durum,
                                 OnayTarihi = k.OnayTarihi,
                                 KayitTarihi = k.KayitTarihi
                             });

            return await query.FirstOrDefaultAsync() ?? new YatirimTalepleriManualListDto();
        }

        public async Task<int> GetCountAsync(Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            IslemTip islemTipi = IslemTip.Deposit;

            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             .AsNoTracking()
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, null, null, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<YatirimTalepleriManualBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            IslemTip islemTipi = IslemTip.Deposit;

            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             //.Include(k => k.Tedarikciler)
                             //.Include(k => k.Bankalar)
                             //.Include(k => k.WebSiteleri)
                             //.Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, null, null, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            if (durum == Durum.Bekliyor)
            {
                query = query.OrderBy(k => k.KayitTarihi);
            }
            else
            {
                query = query.OrderByDescending(k => k.OnayTarihi)
                             .ThenByDescending(k => k.KayitTarihi);
            }

            query = query.Skip((pageNumber - 1) * recordsPerPage)
                         .Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriManualBaseListDto
            {
                Id = k.Id,
                TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                WebSiteTransactionId = k.WebSiteTransactionId,
                AdSoyad = k.AdSoyad,
                UserName = k.UserName,
                UserId = k.UserId,
                HesapNo = k.HesapNo,
                HesapSahibi = k.HesapSahibi,
                Tutar = k.Tutar,
                IsSendToWebSite = k.IsSendToWebSite,
                OnayTarihi = k.OnayTarihi,
                KayitTarihi = k.KayitTarihi
            })
            .ToListAsync();
        }

        private IQueryable<YatirimCekimTalepleriManual> ApplyFilters(IQueryable<YatirimCekimTalepleriManual> query,
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

            if (approveDateStart is not null)
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
    }
}
