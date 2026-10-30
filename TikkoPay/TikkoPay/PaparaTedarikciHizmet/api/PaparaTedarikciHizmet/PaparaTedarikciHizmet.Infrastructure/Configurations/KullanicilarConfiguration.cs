using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
{
    public class KullanicilarConfiguration : BaseModelConfiguration<Kullanicilar>, IEntityTypeConfiguration<Kullanicilar>
    {
        public override void Configure(EntityTypeBuilder<Kullanicilar> builder)
        {
            builder.ToTable("Kullanicilar");

            builder.Property(k => k.AdSoyad).IsRequired().HasMaxLength(250);
            builder.Property(k => k.Eposta).IsRequired().HasMaxLength(250);
            builder.Property(k => k.PasswordHash).IsRequired().HasMaxLength(250);
            builder.Property(k => k.PasswordSalt).IsRequired().HasMaxLength(250);

            builder.HasOne(k => k.Tedarikciler)
                .WithMany(t => t.Kullanicilar)
                .HasForeignKey(k => k.TedarikciId);

            builder.HasOne(k => k.WebSiteleri)
               .WithMany(t => t.Kullanicilar)
               .HasForeignKey(k => k.WebSiteId);

            base.Configure(builder);
        }
    }
}
