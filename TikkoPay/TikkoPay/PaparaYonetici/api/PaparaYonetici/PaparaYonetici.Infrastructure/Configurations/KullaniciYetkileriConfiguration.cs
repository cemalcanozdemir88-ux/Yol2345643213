using PaparaYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaYonetici.Infrastructure.Configurations
{
    public class KullaniciYetkileriConfiguration : BaseModelConfiguration<KullaniciYetkileri>, IEntityTypeConfiguration<KullaniciYetkileri>
    {
        public override void Configure(EntityTypeBuilder<KullaniciYetkileri> builder)
        {
            builder.ToTable("KullaniciYetkileri");


            builder.HasOne(k => k.Kullanicilar)
                .WithMany(k => k.KullaniciYetkileri)
                .HasForeignKey(k => k.KullaniciId);

            builder.HasOne(k => k.Yetkiler)
               .WithMany(k => k.KullaniciYetkileri)
               .HasForeignKey(k => k.YetkiId);

            base.Configure(builder);
        }
    }
}
