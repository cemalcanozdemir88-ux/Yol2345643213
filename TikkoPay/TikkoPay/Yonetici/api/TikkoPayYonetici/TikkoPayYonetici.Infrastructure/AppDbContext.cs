using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> contextOptions, IHttpContextAccessor httpContextAccessor) : base(contextOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // EF core'un çağırdığı orijinal override (varsayılan davranış: setBaseModelValues = true)
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(true, cancellationToken);

        // Ek parametreli versiyon (override değil)
        public async Task<int> SaveChangesAsync(bool setBaseModelValues, CancellationToken cancellationToken = default)
        {
            // HttpContext ve kullanıcı id'si güvenli erişim
            var httpContext = _httpContextAccessor?.HttpContext;
            var nameidentifier = httpContext?.User?.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = null;
            if (Guid.TryParse(nameidentifier, out var parsedGuid))
                kullaniciId = parsedGuid;

            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

            if (setBaseModelValues)
            {
                foreach (var entry in ChangeTracker.Entries<BaseModel>()
                             .Where(k => k.State == EntityState.Added || k.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.Id = Guid.NewGuid();
                        entry.Entity.SilindiMi = false;
                        entry.Entity.KayitTarihi = DateTime.Now;
                        entry.Entity.KaydedenKullaniciId = kullaniciId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.GuncellemeTarihi = DateTime.Now;
                        entry.Entity.GuncelleyenKullaniciId = kullaniciId;
                    }
                }
            }

            // Audit log hazırlığı
            var auditEntries = new List<AuditLog>();

            // Eğer kullaniciId null ise ipLoggingEnabled = false
            bool ipLoggingEnabled = false;
            if (kullaniciId.HasValue)
            {
                ipLoggingEnabled = await Kullanicilar.AsNoTracking()
                                                     .Where(k => k.Id == kullaniciId.Value)
                                                     .Select(k => k.IpLoggingEnabled)
                                                     .SingleOrDefaultAsync(cancellationToken);
            }

            foreach (var entry in ChangeTracker.Entries<BaseModel>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    TableName = entry.Metadata.GetTableName(),
                    EntityId = entry.Entity.Id,
                    KullaniciId = kullaniciId,
                    Tarih = DateTime.Now,
                    IPAddress = ipLoggingEnabled ? ipAddress : string.Empty
                };

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Action = "Added";
                        auditEntry.NewValues = JsonSerializer.Serialize(entry.Entity);
                        break;

                    case EntityState.Modified:
                        auditEntry.Action = "Modified";
                        auditEntry.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                        auditEntry.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());

                        // Soft delete kontrolü
                        if (entry.Entity.SilindiMi && entry.OriginalValues[nameof(BaseModel.SilindiMi)] is false)
                            auditEntry.Action = "Deleted";
                        break;

                    case EntityState.Deleted:
                        auditEntry.Action = "Deleted";
                        auditEntry.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                        break;
                }

                auditEntries.Add(auditEntry);
            }

            // ÖNEMLİ: auditEntries'i context'e ekleyip tek SaveChanges çağrısı ile hepsini kaydediyoruz.
            if (auditEntries.Count > 0)
                AuditLogs.AddRange(auditEntries);

            // Tek SaveChanges çağrısı — EF Core bunu kendi transaction'ı içinde yürütecek
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TedarikciMutabakatlari>(entity =>
            {
                entity.Property(x => x.MutabakatTarihi)
                      .HasConversion(
                          v => v.ToDateTime(TimeOnly.MinValue),   
                          v => DateOnly.FromDateTime(v))          
                      .HasColumnType("date");                   
            });

            modelBuilder.Entity<WebSiteMutabakatlari>(entity =>
            {
                entity.Property(x => x.MutabakatTarihi)
                      .HasConversion(
                          v => v.ToDateTime(TimeOnly.MinValue),
                          v => DateOnly.FromDateTime(v))
                      .HasColumnType("date");
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        [DbFunction("GetTedarikciBakiye", "dbo")]
        public decimal GetTedarikciBakiye(Guid tedarikciId)
        {
            throw new NotImplementedException(); // Sadece EF Core tarafından kullanılacak
        }

        [DbFunction("GetTedarikciBakiyeWithDate", "dbo")]
        public decimal GetTedarikciBakiyeWithDate(Guid tedarikciId, DateTime kayitTarihi)
        {
            throw new NotImplementedException(); // Sadece EF Core tarafından kullanılacak
        }

        [DbFunction("GetTedarikciFastHesabiSayisi", "dbo")]
        public int GetTedarikciFastHesabiSayisi(Guid tedarikciId)
        {
            throw new NotImplementedException(); // Sadece EF Core tarafından kullanılacak
        }

        public DbSet<Affiliates> Affiliates { get; set; }
        public DbSet<Tedarikciler> Teadrikciler { get; set; }
        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<KullaniciOtpValues> KullaniciOtpValues { get; set; }
        public DbSet<WebSiteleri> WebSiteleri { get; set; }
        public DbSet<WebSitePaymentMethods> WebSitePaymentMethods { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Bankalar> Bankalar { get; set; }
        public DbSet<HavaleBankalari> HavaleBankalari { get; set; }
        public DbSet<YatirimCekimTalepleri> YatirimTalepleri { get; set; }
        public DbSet<YatirimCekimTalepleriManual> YatirimTalepleriManual { get; set; }
        public DbSet<KasaHaraketleri> KasaHaraketleri { get; set; }
        public DbSet<WebSiteKasaHaraketleri> WebSiteKasaHaraketleri { get; set; }
        public DbSet<Yetkiler> Yetkiler { get; set; }
        public DbSet<KullaniciYetkileri> KullaniciYetkileri { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SettingsV2> SettingsV2 { get; set; }
        public DbSet<KullaniciBlackLists> KullaniciBlackLists { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ActionRequestLog> ActionRequestLogs { get; set; }
        public DbSet<KullaniciLogin> KullaniciLogins { get; set; }
        public DbSet<KullaniciIpAdresleri> KullaniciIpAdresleri { get; set; }
        public DbSet<WebSiteTedarikcileri> WebSiteTedarikcileri { get; set; }
        public DbSet<WebSiteHavaleBankalari> WebSiteHavaleBankalari { get; set; }
        public DbSet<WebSiteTedarikcileriManual> WebSiteTedarikcileriManual { get; set; }
        public DbSet<TedarikciMutabakatlari> TedarikciMutabakatlari { get; set; }
        public DbSet<WebSiteMutabakatlari> WebSiteMutabakatlari { get; set; }
    }
}
