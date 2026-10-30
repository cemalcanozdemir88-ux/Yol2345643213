using PaparaYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaYonetici.Infrastructure.Configurations
{
    public class WebSiteKasaHaraketleriConfiguration : BaseModelConfiguration<WebSiteKasaHaraketleri>, IEntityTypeConfiguration<WebSiteKasaHaraketleri>
    {
        public override void Configure(EntityTypeBuilder<WebSiteKasaHaraketleri> builder)
        {
            builder.ToTable("WebSiteKasaHaraketleri");

            builder.Property(k => k.Aciklama).IsRequired().HasMaxLength(500);
            builder.Property(k => k.Tutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.DovizKuru).HasColumnType("decimal(13,4)");
            builder.Property(k => k.UsdtMiktar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.UsdtAdres).IsRequired().HasMaxLength(500);
            builder.Property(k => k.UsdtTxtNo).IsRequired().HasMaxLength(500);
            builder.Property(k => k.ResimUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(k => k.WebSiteleri)
                .WithMany(t => t.WebSiteKasaHaraketleriList)
                .HasForeignKey(k => k.WebSitesiId);

            builder.HasOne(k => k.KaydedenKullanici).WithMany(t => t.WebSiteKasaHaraketleriKaydeden).HasForeignKey(k => k.KaydedenKullaniciId);
            builder.HasOne(k => k.GuncelleyenKullanici).WithMany(t => t.WebSiteKasaHaraketleriGuncelleyen).HasForeignKey(k => k.GuncelleyenKullaniciId);

            base.Configure(builder);
        }
    }
}
