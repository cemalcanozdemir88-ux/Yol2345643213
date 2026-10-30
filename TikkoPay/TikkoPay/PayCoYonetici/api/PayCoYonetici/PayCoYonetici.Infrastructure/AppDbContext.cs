using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Infrastructure
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> contextOptions, IHttpContextAccessor httpContextAccessor) : base(contextOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            var nameidentifier = _httpContextAccessor?.HttpContext?.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameidentifier != null ? Guid.Parse(nameidentifier) : null;

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

            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<KullaniciOtpValues> KullaniciOtpValues { get; set; }
        public DbSet<Yetkiler> Yetkiler { get; set; }
        public DbSet<KullaniciYetkileri> KullaniciYetkileri { get; set; }
        public DbSet<YatirimTalepleri> YatirimTalepleri { get; set; }
        public DbSet<CekimTalepleri> CekimTalepleri { get; set; }
        public DbSet<KasaHaraketleri> KasaHaraketleri { get; set; }
    }
}
