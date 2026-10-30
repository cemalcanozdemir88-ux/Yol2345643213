using TedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TedarikciHizmet.Infrastructure.Configurations
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
