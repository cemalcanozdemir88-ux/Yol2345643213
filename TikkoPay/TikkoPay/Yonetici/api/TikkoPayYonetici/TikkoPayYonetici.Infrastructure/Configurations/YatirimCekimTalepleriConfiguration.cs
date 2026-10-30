using TikkoPayYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TikkoPayYonetici.Infrastructure.Configurations
{
    public class YatirimCekimTalepleriConfiguration : BaseModelConfiguration<YatirimCekimTalepleri>, IEntityTypeConfiguration<YatirimCekimTalepleri>
    {
        public override void Configure(EntityTypeBuilder<YatirimCekimTalepleri> builder)
        {
            builder.ToTable("YatirimCekimTalepleri");

            builder.Property(k => k.IdNumber).ValueGeneratedOnAdd().UseIdentityColumn(1000, 1);

            builder.Property(k => k.WebSiteTransactionId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.TalepNo).IsRequired().HasMaxLength(250);
            builder.Property(k => k.PaymentMethodId).IsRequired(false);
            builder.Property(k => k.AdSoyad).IsRequired().HasMaxLength(250);
            builder.Property(k => k.UserName).HasMaxLength(250);
            builder.Property(k => k.UserId).HasMaxLength(250);
            builder.Property(k => k.YontemTipi).IsRequired().HasMaxLength(250);
            builder.Property(k => k.HesapSahibi).IsRequired().HasMaxLength(250);
            builder.Property(k => k.HesapNo).IsRequired().HasMaxLength(250);
            builder.Property(k => k.TedarikciKomisyon).HasColumnType("decimal(13,4)");
            builder.Property(k => k.WebSiteKomisyon).HasColumnType("decimal(13,4)");
            builder.Property(k => k.Tutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.Toplam).HasColumnType("decimal(13,4)");
            builder.Property(k => k.CokluTedarikciyeYonlendirildi).IsRequired(true).HasDefaultValue(false);
            builder.Property(k => k.IsSendToWebSite).IsRequired(true).HasDefaultValue(false);
            builder.Property(k => k.WebSiteCallBackResult).IsRequired(false);
            builder.Property(k => k.IptalAciklama).HasMaxLength(1500);
            builder.Property(k => k.DekontKontrolDurumu).IsRequired(false);
            builder.Property(k => k.DekontKontrolKullaniciId).IsRequired(false);
            builder.Property(k => k.DekontKontrolTarihi).IsRequired(false);

            builder.HasOne(k => k.Tedarikciler)
                .WithMany(t => t.YatirimTalepleri)
                .HasForeignKey(k => k.TedarikciId);

            builder.HasOne(k => k.Bankalar)
                .WithMany(t => t.YatirimTalepleri)
                .HasForeignKey(k => k.BankaId);

            builder.HasOne(k => k.WebSiteleri)
               .WithMany(t => t.YatirimTalepleri)
               .HasForeignKey(k => k.WebSiteId);

            builder.HasOne(k => k.DekontKontrolKullanici).WithMany(t => t.DekontKontrolKullanicilari).HasForeignKey(k => k.DekontKontrolKullaniciId);
            builder.HasOne(k => k.GuncelleyenKullanici).WithMany(t => t.YatirimTalepleri).HasForeignKey(k => k.GuncelleyenKullaniciId);

            base.Configure(builder);
        }
    }
}
