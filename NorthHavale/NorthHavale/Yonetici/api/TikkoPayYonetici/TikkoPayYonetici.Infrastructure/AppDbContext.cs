using System.Text.Json;
using TikkoPayYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TikkoPayYonetici.Infrastructure
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> contextOptions, IHttpContextAccessor httpContextAccessor) : base(contextOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var nameidentifier = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            foreach (var entry in base.ChangeTracker.Entries<BaseModel>().Where(k => k.State == EntityState.Added || k.State == EntityState.Modified))
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

            var auditEntries = new List<AuditLog>();

            var ipLoggingEnabled = await Kullanicilar.AsNoTracking()
                                                     .Where(k => k.Id == kullaniciId)
                                                     .Select(k => k.IpLoggingEnabled)
                                                     .SingleOrDefaultAsync();

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

            using (var transaction = await Database.BeginTransactionAsync())
            {
                var acceptAllChangesOnSuccess = true;

                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);
                await base.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync();

                return result;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

        public DbSet<Affiliates> Affiliates { get; set; }
        public DbSet<Tedarikciler> Teadrikciler { get; set; }
        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<KullaniciOtpValues> KullaniciOtpValues { get; set; }
        public DbSet<WebSiteleri> WebSiteleri { get; set; }
        public DbSet<Bankalar> Bankalar { get; set; }
        public DbSet<HavaleBankalari> HavaleBankalari { get; set; }
        public DbSet<YatirimCekimTalepleri> YatirimTalepleri { get; set; }
        public DbSet<KasaHaraketleri> KasaHaraketleri { get; set; }
        public DbSet<WebSiteKasaHaraketleri> WebSiteKasaHaraketleri { get; set; }
        public DbSet<Yetkiler> Yetkiler { get; set; }
        public DbSet<KullaniciYetkileri> KullaniciYetkileri { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<KullaniciBlackLists> KullaniciBlackLists { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ActionRequestLog> ActionRequestLogs { get; set; }
        public DbSet<KullaniciLogin> KullaniciLogins { get; set; }
        public DbSet<KullaniciIpAdresleri> KullaniciIpAdresleri { get; set; }
    }
}
