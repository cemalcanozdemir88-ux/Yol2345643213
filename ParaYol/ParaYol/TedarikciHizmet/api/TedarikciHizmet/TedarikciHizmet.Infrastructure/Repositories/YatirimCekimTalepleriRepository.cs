using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TedarikciHizmet.Infrastructure.Repositories
{
    public class YatirimCekimTalepleriRepository : GenericRepository<YatirimCekimTalepleri>, IYatirimCekimTalepleriRepository
    {
        public YatirimCekimTalepleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<YatirimTalepleriListDto> GetByIdDtoAsync(Guid id)
        {
            return await _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                             .Select(k => new YatirimTalepleriListDto
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

        private static IQueryable<YatirimCekimTalepleri> ApplyFilters(IQueryable<YatirimCekimTalepleri> query, 
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

        public async Task<int> GetCountAsync(Guid? tedarikciId, IslemTip islemTipi, Durum durum)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                                  .Where(k => k.TedarikciId == tedarikciId &&
                                              k.SilindiMi == false &&
                                              k.IslemTipi == islemTipi &&
                                              k.Durum == durum);
            return await query.CountAsync();
        }

        public async Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
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

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query =  _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                             .Select(k => new YatirimTalepleriListDto
                             {
                                 Id = k.Id,
                                 TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                                 TedarikciId = k.TedarikciId,
                                 TedarikciAdi = k.Tedarikciler.Unvan,
                                 BankaId = k.BankaId,
                                 BankaAdi = k.Bankalar.Baslik,
                                 AdSoyad = k.AdSoyad,
                                 YontemTipi = k.YontemTipi  ,
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

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriListDto
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

        public override Task AddAsync(YatirimCekimTalepleri entity)
        {
            entity.TalepNo = DateTime.Now.ToString("yyyyMMdd");

            return base.AddAsync(entity);
        }

        public async Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                                  .Where(k => k.TedarikciId == tedarikciId &&
                                              k.IslemTipi == IslemTip.Deposit &&
                                              k.Durum == Durum.Onaylandi &&
                                             !k.SilindiMi);
            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            var result = await query.GroupBy(d => d.TedarikciId)
                              .Select(k => new ToplamYatirimTutariDto
                              {
                                  ToplamTutar = k.Sum(s => s.Tutar),
                                  ToplamKomisyon = k.Sum(s => s.TedarikciKomisyon)
                              }).FirstOrDefaultAsync();

            return result ?? new ToplamYatirimTutariDto();
        }

        public async Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                                 .Where(k => k.TedarikciId == tedarikciId &&
                                             k.IslemTipi == IslemTip.Withdrawal &&
                                             k.Durum == Durum.Onaylandi &&
                                            !k.SilindiMi);
            query = ApplyFilters(query, tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            var result = await query.GroupBy(d => d.TedarikciId)
                                  .Select(k => new ToplamCekimTutariDto
                                  {
                                      ToplamTutar = k.Sum(s => s.Tutar),
                                      ToplamKomisyon = k.Sum(s => s.TedarikciKomisyon)
                                  }).FirstOrDefaultAsync();

            return result ?? new ToplamCekimTutariDto();
        }
    }
}
