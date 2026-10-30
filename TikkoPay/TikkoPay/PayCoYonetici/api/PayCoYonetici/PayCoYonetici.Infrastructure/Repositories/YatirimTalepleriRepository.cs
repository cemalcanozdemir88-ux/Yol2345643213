using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayCoYonetici.Core.DTOs.YatirimTalepleri;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Infrastructure.Repositories
{
    public class YatirimTalepleriRepository : GenericRepository<YatirimTalepleri>, IYatirimTalepleriRepository
    {
        public YatirimTalepleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        private IQueryable<YatirimTalepleri> ApplyFilters(IQueryable<YatirimTalepleri> query,
            Guid? webSiteId,
            string searchText,
            DateTime? approveDateStart,
            DateTime? approveDateEnd,
            DateTime? createdDateStart,
            DateTime? createdDateEnd)
        {
            if (webSiteId.HasValue)
            {
                query = query.Where(k => k.WebSiteId == webSiteId);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => (k.TalepNo + k.IdNumber.ToString()).Contains(searchText) ||
                                         k.WebSiteTransactionId.Contains(searchText) ||
                                         k.AdSoyad.Contains(searchText) ||
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

        public async Task<YatirimTalepleriListDto?> GetByIdDtoAsync(Guid id)
        {
            return await _dbContext.Set<YatirimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.Id.Equals(id) && k.SilindiMi == false)
                             .Select(k => new YatirimTalepleriListDto
                             {
                                 Id = k.Id,
                                 WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                                 WebSiteTransactionId = k.WebSiteTransactionId,
                                 AdSoyad = k.AdSoyad,
                                 Tutar = k.Tutar,
                                 Toplam = k.Toplam,
                                 IptalAciklama = k.IptalAciklama,
                                 OnayTarihi = k.OnayTarihi,
                                 KayitTarihi = k.KayitTarihi
                             }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            return await query.OrderByDescending(k => k.KayitTarihi)
                             .Select(k => new YatirimTalepleriListDto
                             {
                                 Id = k.Id,
                                 TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                                 WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                                 WebSiteTransactionId = k.WebSiteTransactionId,
                                 AdSoyad = k.AdSoyad,
                                 UserName = k.UserName,
                                 UserId = k.UserId,
                                 Tutar = k.Tutar,
                                 Toplam = k.Toplam,
                                 IptalAciklama = k.IptalAciklama,
                                 OnayTarihi = k.OnayTarihi,
                                 GuncellemeTarihi = k.GuncellemeTarihi,
                                 KayitTarihi = k.KayitTarihi
                             })
                             .ToListAsync();
        }

        public async Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var query = _dbContext.Set<YatirimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new YatirimTalepleriListDto
            {
                Id = k.Id,
                TalepNoWithFormated = $"{k.TalepNo}{k.IdNumber}",
                WebSiteUrl = k.WebSiteleri.WebSiteUrl,
                WebSiteTransactionId = k.WebSiteTransactionId,
                AdSoyad = k.AdSoyad,
                UserName = k.UserName,
                UserId = k.UserId,
                Tutar = k.Tutar,
                Toplam = k.Toplam,
                IptalAciklama = k.IptalAciklama,
                OnayTarihi = k.OnayTarihi,
                GuncellemeTarihi = k.GuncellemeTarihi,
                KayitTarihi = k.KayitTarihi
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<YatirimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var query = _dbContext.Set<YatirimTalepleri>()
                                  .Include(k => k.WebSiteleri)
                                  .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
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

        public async Task<int> GetCountAsync(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimTalepleri>()
                             .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<YatirimTalepleri>()
                                  .Where(k => k.SilindiMi == false);
            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var result = await query.ToListAsync();

            return new ToplamYatirimTutariDto
            {
                ToplamTutar = result.Sum(k => k.Tutar),
                ToplamKomisyon = result.Sum(k => k.WebSiteKomisyon)
            };
        }
    }
}
