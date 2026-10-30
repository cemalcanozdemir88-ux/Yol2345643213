using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class YatirimCekimTalepleriManualRepository : GenericRepository<YatirimCekimTalepleriManual>, IYatirimCekimTalepleriManualRepository
    {
        public YatirimCekimTalepleriManualRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<YatirimTalepleriManualListDto> GetByIdDtoAsync(Guid id)
        {
            return await _dbContext.Set<YatirimCekimTalepleriManual>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                             .Select(k => new YatirimTalepleriManualListDto
                             {
                                 Id = k.Id,
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
                             }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<YatirimTalepleriManualListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
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
                                 HesapSahibi = (islemTipi == IslemTip.Withdrawal && durum == Durum.Bekliyor) ? string.Empty : k.HesapSahibi,
                                 HesapNo = k.HesapNo,
                                 HesapNoBanka = k.HesapNoBanka,
                                 Tutar = k.Tutar,
                                 TedarikciKomisyon = k.TedarikciKomisyon,
                                 Toplam = k.Toplam,
                                 IslemTipi = k.IslemTipi,
                                 IptalAciklama = k.IptalAciklama,
                                 Durum = k.Durum,
                                 OnayTarihi = k.OnayTarihi,
                                 GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                                 KayitTarihi = k.KayitTarihi
                             })
                             .ToListAsync();

        }

        public async Task<IEnumerable<YatirimTalepleriManualListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            if (durum == Durum.Bekliyor)
            {
                query = query.OrderBy(k => k.KayitTarihi);
            }
            else
            {
                query = query.OrderByDescending(k => k.KayitTarihi);
            }

            query = query.Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriManualListDto
            {
                Id = k.Id,
                TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                TedarikciId = k.TedarikciId,
                TedarikciAdi = k.Tedarikciler.Unvan,
                BankaId = k.BankaId,
                BankaAdi = k.Bankalar.Baslik,
                AdSoyad = k.AdSoyad,
                YontemTipi = k.YontemTipi,
                HesapSahibi = (islemTipi == IslemTip.Withdrawal && durum == Durum.Bekliyor) ? string.Empty : k.HesapSahibi,
                HesapNo = k.HesapNo,
                HesapNoBanka = k.HesapNoBanka,
                Tutar = k.Tutar,
                TedarikciKomisyon = k.TedarikciKomisyon,
                Toplam = k.Toplam,
                IslemTipi = k.IslemTipi,
                IptalAciklama = k.IptalAciklama,
                Durum = k.Durum,
                OnayTarihi = k.OnayTarihi,
                GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                KayitTarihi = k.KayitTarihi
            })
                             .ToListAsync();

        }


        public async Task<int> GetCountAsync(Guid? tedarikciId, IslemTip islemTipi, Durum durum)
        {
            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                                  .Where(k => k.TedarikciId == tedarikciId &&
                                              k.SilindiMi == false &&
                                              k.IslemTipi == islemTipi &&
                                              k.Durum == durum);
            return await query.CountAsync();
        }

        public async Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleriManual>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        private static IQueryable<YatirimCekimTalepleriManual> ApplyFilters(IQueryable<YatirimCekimTalepleriManual> query,
            Guid? tedarikciId,
            Guid? bankaId,
            string searchText,
            DateTime? approveDateStart,
            DateTime? approveDateEnd,
            DateTime? createdDateStart,
            DateTime? createdDateEnd)
        {
            query = query.Where(k => k.TedarikciId == tedarikciId);

            if (bankaId.HasValue)
            {
                query = query.Where(k => k.BankaId == bankaId);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => (k.TalepNo + k.IdNumber.ToString()).Contains(searchText) ||
                                         k.WebSiteTransactionId.Contains(searchText) ||
                                         k.AdSoyad.Contains(searchText) ||
                                         k.YontemTipi.Contains(searchText) ||
                                         k.HesapSahibi.Contains(searchText) ||
                                         k.HesapNo.Contains(searchText));
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
