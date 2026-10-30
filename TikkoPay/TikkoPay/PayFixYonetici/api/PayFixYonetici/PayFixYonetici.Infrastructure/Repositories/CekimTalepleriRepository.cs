using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayFixYonetici.Core.DTOs.CekimTalepleri;
using PayFixYonetici.Core.Enums;
using PayFixYonetici.Core.Interfaces;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Repositories
{
    public class CekimTalepleriRepository : GenericRepository<CekimTalepleri>, ICekimTalepleriRepository
    {
        public CekimTalepleriRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<int> GetCountAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<CekimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.SilindiMi == false &&
                                         k.Durum == durum);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<CekimTalepleriListDto>> GetAllAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var query = _dbContext.Set<CekimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.SilindiMi == false &&
                                         k.Durum == durum);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new CekimTalepleriListDto
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

        public async Task<IEnumerable<CekimTalepleriBaseListDto>> GetAllForWebSiteAsync(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var query = _dbContext.Set<CekimTalepleri>()
                             .Include(k => k.WebSiteleri)
                             .Where(k => k.SilindiMi == false &&
                                         k.Durum == durum);

            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            query = query.OrderByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

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

        public async Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var query = _dbContext.Set<CekimTalepleri>()
                             .Where(k => k.SilindiMi == false &&
                                         k.Durum == true &&
                                         !k.SilindiMi);
            query = ApplyFilters(query, webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var result = await query.ToListAsync();

            return new ToplamCekimTutariDto
            {
                ToplamTutar = result.Sum(k => k.Tutar),
                ToplamKomisyon = result.Sum(k => k.WebSiteKomisyon)
            };
        }

        private IQueryable<CekimTalepleri> ApplyFilters(IQueryable<CekimTalepleri> query,
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
    }
}
