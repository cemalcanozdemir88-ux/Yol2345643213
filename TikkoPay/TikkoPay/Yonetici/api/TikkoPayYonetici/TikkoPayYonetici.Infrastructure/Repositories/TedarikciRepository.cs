using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.DTOs.Tedarikciler;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class TedarikciRepository : GenericRepository<Tedarikciler>, ITedarikciRepository
    {
        private readonly DbSet<Tedarikciler> _dbSet;
        private readonly DbSet<YatirimCekimTalepleri> _dbSetYatirimTaleperi;
        private readonly DbSet<KasaHaraketleri> _dbSetKasaHaraketleri;

        public TedarikciRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _dbSet = _dbContext.Set<Tedarikciler>();
            _dbSetYatirimTaleperi = _dbContext.Set<YatirimCekimTalepleri>();
            _dbSetKasaHaraketleri = _dbContext.Set<KasaHaraketleri>();
        }

        private IQueryable<Tedarikciler> ApplyFilters(IQueryable<Tedarikciler> query, bool? bankaHavale, bool? otoPuan, bool? durum)
        {
            if (bankaHavale.HasValue)
            {
                query = query.Where(k => k.BankaHavale == bankaHavale.Value);
            }

            if (otoPuan.HasValue)
            {
                query = query.Where(k => k.OtoPuan == otoPuan.Value);
            }

            if (durum.HasValue)
            {
                query = query.Where(k => k.Durum == durum.Value);
            }

            return query;
        }

        public async Task<int> GetCountAsync(bool? bankaHavale, bool? otoPuan, bool? durum)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            query = ApplyFilters(query, bankaHavale, otoPuan, durum);

            int totalRecords = await query.CountAsync();

            return totalRecords;
        }

        public async Task<IEnumerable<TedarikcilerListDto>> GetAllDtoAsync(bool? bankaHavale, bool? otoPuan, bool? durum, int pageNumber = 1, int recordsPerPage = 10)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            query = ApplyFilters(query, bankaHavale, otoPuan, durum);
            query = query.OrderBy(k => k.Unvan).Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            return await query.Select(k => new TedarikcilerListDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                BankaHavale = k.BankaHavale,
                YaririmKomisyonOrani = k.YaririmKomisyonOrani,
                CekimKomisyonOrani = k.CekimKomisyonOrani,
                TedarikcidenAlinanOdemeKomisyonOrani = k.TedarikcidenAlinanOdemeKomisyonOrani,
                MinYatirimTutari = k.MinYatirimTutari,
                YatirimPuani = k.YatirimPuani,
                HesapNoGosterimSayisi = k.HesapNoGosterimSayisi,
                FastHesapSayisi = _dbContext.GetTedarikciFastHesabiSayisi(k.Id),
                AnlikBakiye = _dbContext.GetTedarikciBakiye(k.Id),
                BakiyeLimitor = k.BakiyeLimitor,
                OtoPuan = k.OtoPuan,
                Durum = k.Durum
            }).ToListAsync();
        }

        public async Task<List<TedarikcilerSelectList>> GetSelectListAsync()
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);

            return await query.OrderBy(k => k.Unvan).Select(k => new TedarikcilerSelectList
            {
                Id = k.Id,
                Unvan = $"{k.Unvan} - {(k.Durum ? "Aktif" : "Pasif")}",
            }).ToListAsync();
        }

        public async Task<List<TedarikcilerSelectListWithBakiyeDto>> GetSelectListWithBakiyeAsync()
        {
            var query = _dbSet.Where(k => k.Durum == true && k.SilindiMi == false);

            return await query.OrderBy(k => k.Unvan).Select(k => new TedarikcilerSelectListWithBakiyeDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                AnlikBakiye = _dbContext.GetTedarikciBakiye(k.Id)
            }).ToListAsync();
        }

        public async Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId, int? tedarikciDurum)
        {
            var query = _dbSet.Where(k => k.SilindiMi == false);
            if (tedarikciId.HasValue)
            {
                query = query.Where(k => k.Id == tedarikciId.Value);
            }

            if (tedarikciDurum.HasValue)
            {
                if (tedarikciDurum.Value == 1)
                {
                    query = query.Where(k => k.Durum == true);
                }
                else if (tedarikciDurum.Value == 0)
                {
                    query = query.Where(k => k.Durum == false);
                }
            }

            return await query
                         .OrderBy(k => k.Unvan)
                         .Select(k => new TedarikciBakiyeListDto
            {
                Id = k.Id,
                Unvan = k.Unvan,
                TedarikciDurum = k.Durum,
                BakiyeLimitor = k.BakiyeLimitor,
                ToplamYatirim = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id && 
                                            y.Durum == Core.Enums.Durum.Onaylandi && 
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit && 
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamYatirimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.TedarikciKomisyon),
                ToplamCekim = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar),
                ToplamCekimKomisyon = _dbSetYatirimTaleperi
                                .Where(y => y.TedarikciId == k.Id &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                            y.SilindiMi == false)
                                .Sum(k => k.TedarikciKomisyon),
                ToplamKasaHaraketiGiris = _dbSetKasaHaraketleri
                                          .Where(y => y.TedarikciId == k.Id && 
                                                 y.Durum == Core.Enums.Durum.Onaylandi && 
                                                 (y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.GirisManuel || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanCikis) && 
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar),
                ToplamKasaHaraketiCikis = _dbSetKasaHaraketleri
                                          .Where(y => y.TedarikciId == k.Id &&
                                                 y.Durum == Core.Enums.Durum.Onaylandi &&
                                                 (y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.CikisManuel || y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanGiris) &&
                                                 y.SilindiMi == false)
                                          .Sum(k => k.Tutar)

            }).ToListAsync();
        }

        public async Task<bool> StartUpdateTedarikciBakiyeJobAsync()
        {
            // KODUNUZUN ÇALIŞMA SÜRESİ UZUN OLABİLİR (Bkz. Aşağıdaki Timeout Uyarısı)
            // EF Core varsayılan olarak 30 saniye sonra zaman aşımına uğrar. Süreyi artırıyoruz:
            _dbContext.Database.SetCommandTimeout(180); // 3 dakika (İhtiyaca göre ayarlayın)

            string sqlScript = @"
        BEGIN TRY
            DECLARE @HesaplamaTarihi DATETIME = DATEADD(HOUR, -3, GETDATE())
            DECLARE @RetryCount INT = 0, @MaxRetries INT = 3

            WHILE @RetryCount < @MaxRetries
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION

                    ;WITH SortedTedarikciler AS (
                        SELECT TOP 100 PERCENT Id 
                        FROM Tedarikciler 
                        ORDER BY Id
                    )
                    UPDATE T WITH (UPDLOCK, ROWLOCK)
                    SET 
                        AnlikBakiye = dbo.GetTedarikciBakiyeWithDate(T.Id, @HesaplamaTarihi),
                        AnlikBakiyeHesaplamaTarihi = @HesaplamaTarihi
                    FROM SortedTedarikciler AS S
                    INNER JOIN Tedarikciler T ON S.Id = T.Id

                    COMMIT TRANSACTION
                    BREAK 
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
                    
                    DECLARE @ErrorNumber INT = ERROR_NUMBER()
                    IF @ErrorNumber IN (1205, 1222)
                    BEGIN
                        SET @RetryCount += 1
                        IF @RetryCount < @MaxRetries
                            WAITFOR DELAY '00:00:01'
                    END
                    ELSE
                        THROW 
                END CATCH
            END
        END TRY
        BEGIN CATCH
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
            DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
            DECLARE @ErrorState INT = ERROR_STATE()
            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH";

            try
            {
                // Komutu çalıştırır ve etkilenen satır sayısını döner
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlScript);
                return true;
            }
            catch (Exception ex)
            {
                // SQL içindeki RAISERROR buraya Exception olarak düşer
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                throw;
            }
        }
    }
}
