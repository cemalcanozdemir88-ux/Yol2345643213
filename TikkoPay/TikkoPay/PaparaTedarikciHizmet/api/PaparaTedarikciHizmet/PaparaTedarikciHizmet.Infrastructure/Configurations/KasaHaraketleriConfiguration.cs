using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
{
    public class KasaHaraketleriConfiguration : BaseModelConfiguration<KasaHaraketleri>, IEntityTypeConfiguration<KasaHaraketleri>
    {
        public override void Configure(EntityTypeBuilder<KasaHaraketleri> builder)
        {
            builder.ToTable("KasaHaraketleri");

            builder.Property(k => k.Aciklama).IsRequired().HasMaxLength(500);
            builder.Property(k => k.Tutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.DovizKuru).HasColumnType("decimal(13,4)");
            builder.Property(k => k.UsdtMiktar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.UsdtAdres).IsRequired().HasMaxLength(500);
            builder.Property(k => k.UsdtTxtNo).IsRequired().HasMaxLength(500);
            builder.Property(k => k.ResimUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(k => k.Tedarikciler)
                .WithMany(t => t.KasaHaraketleriList)
                .HasForeignKey(k => k.TedarikciId);

            builder.HasOne(k => k.KaydedenKullanici).WithMany(t => t.KasaHaraketleriKaydeden).HasForeignKey(k => k.KaydedenKullaniciId);
            builder.HasOne(k => k.GuncelleyenKullanici).WithMany(t => t.KasaHaraketleriGuncelleyen).HasForeignKey(k => k.GuncelleyenKullaniciId);

            base.Configure(builder);
        }
    }
}
