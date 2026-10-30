using TikkoPayYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TikkoPayYonetici.Infrastructure.Configurations
{
    public class TedarikcilerConfiguration : BaseModelConfiguration<Tedarikciler>, IEntityTypeConfiguration<Tedarikciler>
    {
        public override void Configure(EntityTypeBuilder<Tedarikciler> builder)
        {
            builder.ToTable("Tedarikciler");
            
            builder.Property(k => k.Unvan).IsRequired().HasMaxLength(250);
            builder.Property(k => k.ToplamYatirim).HasColumnType("decimal(13,4)");
            builder.Property(k => k.ToplamYatirimKomisyon).HasColumnType("decimal(13,4)");
            builder.Property(k => k.ToplamCekim).HasColumnType("decimal(13,4)");
            builder.Property(k => k.ToplamCekimKomisyon).HasColumnType("decimal(13,4)");
            builder.Property(k => k.ToplamKasaHaraketiGiris).HasColumnType("decimal(13,4)");
            builder.Property(k => k.ToplamKasaHaraketiCikis).HasColumnType("decimal(13,4)");
            builder.Property(k => k.AnlikBakiye).HasColumnType("decimal(13,4)");

            base.Configure(builder);
        }
    }
}
