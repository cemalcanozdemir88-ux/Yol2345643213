using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
{
    public class HavaleBankalariConfiguration : BaseModelConfiguration<HavaleBankalari>, IEntityTypeConfiguration<HavaleBankalari>
    {
        public override void Configure(EntityTypeBuilder<HavaleBankalari> builder)
        {
            builder.ToTable("HavaleBankalari");

            builder.Property(k => k.HesapSahibi).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.HesapNo).IsRequired().HasMaxLength(250);
            builder.Property(k => k.HesapMinTutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.HesapMaxTutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.MaxYatirimAdedi).HasColumnType("int").HasDefaultValue(0);
            builder.Property(k => k.MaxYatirimToplami).HasColumnType("decimal(13,4)").HasDefaultValue(0);
            builder.Property(k => k.OnaylananYatirimAdedi).HasColumnType("int").HasDefaultValue(0);
            builder.Property(k => k.OnaylananYatirimAdediTemp).HasColumnType("int").HasDefaultValue(0);
            builder.Property(k => k.OnaylananYatirimToplami).HasColumnType("decimal(13,4)").HasDefaultValue(0);

            builder.HasOne(k => k.Tedarikciler)
                .WithMany(t => t.HavaleBankalari)
                .HasForeignKey(k => k.TedarikciId);

            builder.HasOne(k => k.Bankalar)
                .WithMany(t => t.HavaleBankalari)
                .HasForeignKey(k => k.BankaId);

            base.Configure(builder);
        }
    }
}
