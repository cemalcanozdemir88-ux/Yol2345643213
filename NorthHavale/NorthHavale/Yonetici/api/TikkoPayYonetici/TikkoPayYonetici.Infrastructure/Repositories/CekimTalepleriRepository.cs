using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class CekimTalepleriRepository : GenericRepository<YatirimCekimTalepleri>, ICekimTalepleriRepository
    {
        public CekimTalepleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<CekimTalepleriListDto?> GetByIdDtoAsync(Guid id)
        {
            return await _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                             .Select(k => new CekimTalepleriListDto
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

        private IQueryable<YatirimCekimTalepleri> ApplyFilters(
            IQueryable<YatirimCekimTalepleri> query,
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
            else
            {
                query = query.Where(k => k.TopId == null);
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

        public async Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            IslemTip islemTipi = IslemTip.Withdraw;

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

        public async Task<IEnumerable<CekimTalepleriListDto>> GetByTopId(Guid? topId, bool showWebSite)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.TopId == topId);

            
            return await query.OrderByDescending(k => k.OnayTarihi)
                              .ThenByDescending(k => k.KayitTarihi)
                             .Select(k => new CekimTalepleriListDto
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
                                 HesapNo = k.HesapNo,
                                 Tutar = k.Tutar,
                                 TedarikciKomisyon = k.TedarikciKomisyon,
                                 Toplam = k.Toplam,
                                 IslemTipi = k.IslemTipi,
                                 IptalAciklama = k.IptalAciklama,
                                 Durum = k.Durum,
                                 YonlendirilmeTarihi = k.YonlendirilmeTarihi,
                                 OnayTarihi = k.OnayTarihi,
                                 GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                                 GuncellemeTarihi = k.GuncellemeTarihi,
                                 KayitTarihi = k.KayitTarihi
                             })
                             .ToListAsync();
        }

        public async Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            IslemTip islemTipi = IslemTip.Withdraw;

            var query =  _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.OnayTarihi)
                              .ThenByDescending(k => k.KayitTarihi)
                             .Select(k => new CekimTalepleriListDto
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

        public async Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, bool showWebSite, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            IslemTip islemTipi = IslemTip.Withdraw;

            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.OnayTarihi)
                         .ThenByDescending(k => k.KayitTarihi)
                         .Skip((pageNumber - 1) * recordsPerPage)
                         .Take(recordsPerPage);

            return await query.Select(k => new CekimTalepleriListDto
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
                CokluTedarikciyeYonlendirildi = k.CokluTedarikciyeYonlendirildi,
                OnayTarihi = k.OnayTarihi,
                GuncelleyenKullanici = k.GuncelleyenKullaniciId.HasValue ? k.GuncelleyenKullanici.AdSoyad : string.Empty,
                GuncellemeTarihi = k.GuncellemeTarihi,
                YonlendirenKullanici = k.YonlendirenKullaniciId.HasValue ? k.YonlendirenKullanici.AdSoyad : string.Empty,
                YonlendirilmeTarihi = k.YonlendirilmeTarihi,
                KayitTarihi = k.KayitTarihi
            })
            .ToListAsync();
        }

        public async Task<GetCekimTalepleriResponse> GetTumCekimTalepleriWithSpAsync(
    Guid? tedarikciId,
    Guid? bankaId,
    Guid? webSiteId,
    string searchText,
    Durum durum,
    DateTime? approvedDateStart,
    DateTime? approvedDateEnd,
    DateTime? createdDateStart,
    DateTime? createdDateEnd,
    int orderBy = 1,
    int pageNumber = 1,
    int recordsPerPage = 20)
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_GetTumCekimTalepleri";
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.Add(new SqlParameter("@TedarikciId", tedarikciId ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@BankaId", bankaId ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@WebSiteId", webSiteId ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@SearchText", searchText ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Durum", durum));
                command.Parameters.Add(new SqlParameter("@ApprovedDateStart", approvedDateStart ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ApprovedDateEnd", approvedDateEnd ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CreatedDateStart", createdDateStart ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CreatedDateEnd", createdDateEnd ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@OrderBy", orderBy));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
                command.Parameters.Add(new SqlParameter("@RecordsPerPage", recordsPerPage));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // İlk sonuç seti: Toplam kayıt sayısı
                    int totalRecords = 0;
                    if (await reader.ReadAsync())
                    {
                        totalRecords = reader.GetInt32(0);
                    }

                    // İkinci sonuç seti: Kayıtlar
                    var records = new List<CekimTalepleriListDto>();
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CekimTalepleriListDto
                            {
                                Id = reader.GetGuid("Id"),
                                TalepNoWithFormated = reader.GetString("TalepNoWithFormated"),
                                PaymentMethodId = reader.GetInt32("PaymentMethodId"),
                                TedarikciId = reader.IsDBNull(reader.GetOrdinal("TedarikciId")) ? null : reader.GetGuid("TedarikciId"), 
                                TedarikciAdi = reader.IsDBNull(reader.GetOrdinal("TedarikciAdi")) ? string.Empty : reader.GetString("TedarikciAdi"), 
                                WebSiteUrl = reader.GetString("WebSiteUrl"),
                                WebSiteTransactionId = reader.GetString("WebSiteTransactionId"),
                                WithdrawTransactionId = reader.IsDBNull(reader.GetOrdinal("WithdrawTransactionId")) ? null : reader.GetGuid("WithdrawTransactionId"),
                                AdSoyad = reader.GetString("AdSoyad"),
                                UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString("UserName"),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetString("UserId"),
                                YontemTipi = reader.GetString("YontemTipi"),
                                HesapSahibi = reader.GetString("HesapSahibi"),
                                HesapNo = reader.GetString("HesapNo"),
                                Tutar = reader.GetDecimal("Tutar"),
                                TedarikciKomisyon = reader.GetDecimal("TedarikciKomisyon"),
                                Toplam = reader.GetDecimal("Toplam"),
                                IslemTipi = (IslemTip)reader.GetInt32("IslemTipi"),
                                IptalAciklama = durum == Durum.Onaylandi ? string.Empty : (reader.IsDBNull(reader.GetOrdinal("IptalAciklama")) ? string.Empty : reader.GetString("IptalAciklama")),
                                Durum = (Durum)reader.GetInt32("Durum"),
                                OnayTarihi = reader.IsDBNull(reader.GetOrdinal("OnayTarihi")) ? null : reader.GetDateTime("OnayTarihi"),
                                GuncelleyenKullanici = reader.IsDBNull(reader.GetOrdinal("GuncelleyenKullanici")) ? string.Empty : reader.GetString("GuncelleyenKullanici"),
                                GuncellemeTarihi = reader.IsDBNull(reader.GetOrdinal("GuncellemeTarihi")) ? null : reader.GetDateTime("GuncellemeTarihi"),
                                YonlendirenKullanici = reader.IsDBNull(reader.GetOrdinal("YonlendirenKullanici")) ? string.Empty : reader.GetString("YonlendirenKullanici"),
                                YonlendirilmeTarihi = reader.IsDBNull(reader.GetOrdinal("YonlendirilmeTarihi")) ? null : reader.GetDateTime("YonlendirilmeTarihi"),
                                CokluTedarikciyeYonlendirildi = reader.GetBoolean("CokluTedarikciyeYonlendirildi"),
                                KayitTarihi = reader.GetDateTime("KayitTarihi")
                            });
                        }
                    }

                    return new GetCekimTalepleriResponse
                    {
                        Count = totalRecords,
                        CekimTalepleriListDto = records
                    };
                }
            }
        }

        public async Task<IEnumerable<CekimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            IslemTip islemTipi = IslemTip.Withdraw;

            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Include(k => k.Tedarikciler)
                             .Include(k => k.Bankalar)
                             .Include(k => k.WebSiteleri)
                             .Include(k => k.GuncelleyenKullanici)
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == islemTipi &&
                                         k.Durum == durum);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.OnayTarihi)
                         .ThenByDescending(k => k.KayitTarihi)
                         .Skip((pageNumber - 1) * recordsPerPage)
                         .Take(recordsPerPage);

            return await query.Select(k => new CekimTalepleriBaseListDto
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

        public async Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                             .Where(k => k.SilindiMi == false &&
                                         k.IslemTipi == IslemTip.Withdraw &&
                                         k.Durum == Durum.Onaylandi);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            var result = await query
                                .GroupBy(_ => 1)
                                .Select(g => new ToplamCekimTutariDto
                                {
                                    ToplamTutar = g.Sum(k => k.Tutar),
                                    TedarikciToplamKomisyon = g.Sum(k => k.TedarikciKomisyon),
                                    WebsiteToplamKomisyon = g.Sum(k => k.WebSiteKomisyon)
                                })
                                .FirstOrDefaultAsync();

            return result ?? new ToplamCekimTutariDto();
        }

        public async Task<OnaySuresiDto> GetOrtalamaOnaySuresi(Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimCekimTalepleri>()
                                  .Where(k => k.SilindiMi == false &&
                                              k.IslemTipi == IslemTip.Withdraw &&
                                              k.Durum == Durum.Onaylandi &&
                                              k.OnayTarihi.HasValue);

            query = ApplyFilters(query, tedarikciId, bankaId, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            var items = await query.Select(k => EF.Functions.DateDiffSecond(k.KayitTarihi, k.OnayTarihi!.Value))
                                           .ToListAsync();

            var ortalamaSaniye = items.Any() ? items.Average() : 0;

            var timespan = TimeSpan.FromSeconds(ortalamaSaniye);

            var dto = new OnaySuresiDto
            {
                Saat = timespan.Hours,
                Dakika = timespan.Minutes,
                Saniye = timespan.Seconds
            };

            return dto;
        }

        public async Task DeleteByTopId(Guid topId)
        {
            var items = await _dbContext.Set<YatirimCekimTalepleri>().Where(k => k.TopId == topId).ToListAsync();
            foreach (var item in items)
            {
                item.SilindiMi = true;
            }
        }
    }
}
