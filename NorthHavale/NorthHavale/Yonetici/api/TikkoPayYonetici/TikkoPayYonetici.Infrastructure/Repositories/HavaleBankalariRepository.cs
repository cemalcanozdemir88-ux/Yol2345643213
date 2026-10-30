using TikkoPayYonetici.Core.DTOs.HavaleBankalari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class HavaleBankalariRepository : GenericRepository<HavaleBankalari>, IHavaleBankalariRepository
    {
        public HavaleBankalariRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<HavaleBankalariListDto?> GetByIdDtoAsync(Guid id)
        {
            var query = base._dbContext.Set<HavaleBankalari>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.Bankalar)
                .Where(k => k.Id.Equals(id) && k.SilindiMi == false);

            if (!IsSuperAdmin())
            {
                query = query.Where(k => k.TedarikciId == GetTedarikciId());
            }

            return await query.Select(h => new HavaleBankalariListDto
            {
                Id = h.Id,
                TedarikciId = h.TedarikciId,
                BankaId = h.BankaId,
                TedarikciAdi = h.Tedarikciler.Unvan,
                BankaAdi = h.Bankalar.Baslik,
                HesapSahibi = h.HesapSahibi,
                HesapNo = h.HesapNo,
                HesapMinTutar = h.HesapMinTutar,
                HesapMaxTutar = h.HesapMaxTutar,
                MaxYatirimAdedi = h.MaxYatirimAdedi,
                MaxYatirimToplami = h.MaxYatirimToplami,
                OnaylananYatirimAdedi = h.OnaylananYatirimAdedi,
                OnaylananYatirimAdediTemp = h.OnaylananYatirimAdediTemp,
                OnaylananYatirimToplami = h.OnaylananYatirimToplami,
                Durum = h.Durum
            })
            .FirstOrDefaultAsync();

        }

        private IQueryable<HavaleBankalari> ApplyFilters(IQueryable<HavaleBankalari> query, Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter)
        {
            if (tedarikciId is not null)
            {
                query = query.Where(k => k.TedarikciId == tedarikciId);
            }

            if (bankaId is not null)
            {
                query = query.Where(k => k.BankaId == bankaId);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(k => k.HesapSahibi.Contains(searchText) ||
                                         k.HesapNo.Contains(searchText));
            }

            if(havaleBankalariDurumFilter != HavaleBankalariDurumFilter.All)
            {
                var durum = havaleBankalariDurumFilter == HavaleBankalariDurumFilter.Active;
                query = query.Where(k => k.Durum == durum);
            }

            return query;
        }

        public async Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter)
        {
            if (!IsSuperAdmin())
            {
                tedarikciId = base.GetTedarikciId();
            }

            var query = base._dbContext.Set<HavaleBankalari>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.Bankalar)
                .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, havaleBankalariDurumFilter);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter)
        {
            if (!IsSuperAdmin())
            {
                tedarikciId = base.GetTedarikciId();
            }

            var query = base._dbContext.Set<HavaleBankalari>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.Bankalar)
                .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, havaleBankalariDurumFilter);

            return await query.OrderBy(k => k.TedarikciId).ThenBy(k => k.BankaId).ThenByDescending(k => k.KayitTarihi)
                    .Select(h => new HavaleBankalariListDto
                    {
                        Id = h.Id,
                        TedarikciId = h.TedarikciId,
                        BankaId = h.BankaId,
                        TedarikciAdi = h.Tedarikciler.Unvan,
                        BankaAdi = h.Bankalar.Baslik,
                        HesapSahibi = h.HesapSahibi,
                        HesapNo = h.HesapNo,
                        HesapMinTutar = h.HesapMinTutar,
                        HesapMaxTutar = h.HesapMaxTutar,
                        MaxYatirimAdedi = h.MaxYatirimAdedi,
                        MaxYatirimToplami = h.MaxYatirimToplami,
                        OnaylananYatirimAdedi = h.OnaylananYatirimAdedi,
                        OnaylananYatirimAdediTemp = h.OnaylananYatirimAdediTemp,
                        OnaylananYatirimToplami = h.OnaylananYatirimToplami,
                        Durum = h.Durum
                    })
                .ToListAsync();

        }

        public async Task<IEnumerable<HavaleBankalariListDto>> GetAllDtoAsync(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber = 1, int recordsPerPage = 10)
        {
            if (!IsSuperAdmin())
            {
                tedarikciId = base.GetTedarikciId();
            }

            var query = base._dbContext.Set<HavaleBankalari>()
                .Include(k => k.Tedarikciler)
                .Include(k => k.Bankalar)
                .Where(k => k.SilindiMi == false);

            query = ApplyFilters(query, tedarikciId, bankaId, searchText, havaleBankalariDurumFilter);

            query = query.OrderBy(k => k.TedarikciId).ThenBy(k => k.BankaId).ThenByDescending(k => k.KayitTarihi).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(h => new HavaleBankalariListDto
            {
                Id = h.Id,
                TedarikciId = h.TedarikciId,
                BankaId = h.BankaId,
                TedarikciAdi = h.Tedarikciler.Unvan,
                BankaAdi = h.Bankalar.Baslik,
                HesapSahibi = h.HesapSahibi,
                HesapNo = h.HesapNo,
                HesapMinTutar = h.HesapMinTutar,
                HesapMaxTutar = h.HesapMaxTutar,
                MaxYatirimAdedi = h.MaxYatirimAdedi,
                MaxYatirimToplami = h.MaxYatirimToplami,
                OnaylananYatirimAdedi = h.OnaylananYatirimAdedi,
                OnaylananYatirimAdediTemp = h.OnaylananYatirimAdediTemp,
                OnaylananYatirimToplami = h.OnaylananYatirimToplami,
                Durum = h.Durum
            })
                .ToListAsync();

        }

        public override Task AddAsync(HavaleBankalari entity)
        {
            if (!IsSuperAdmin())
            {
                var tedarikciId = base.GetTedarikciId();
                entity.TedarikciId = tedarikciId;
            }

            return base.AddAsync(entity);
        }

        public override void Update(HavaleBankalari entity, object dto)
        {
            if (!IsSuperAdmin())
            {
                var tedarikciId = base.GetTedarikciId();
                entity.TedarikciId = tedarikciId;
            }

            base.Update(entity, dto);
        }
    }
}
