using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Configurations
{
    public class KullaniciIpAdresleriConfiguration : BaseModelConfiguration<KullaniciIpAdresleri>, IEntityTypeConfiguration<KullaniciIpAdresleri>
    {
        public override void Configure(EntityTypeBuilder<KullaniciIpAdresleri> builder)
        {
            builder.ToTable("KullaniciIpAdresleri");

            builder.HasOne(k => k.Kullanicilar).WithMany(t => t.KullaniciIpAdresleri).HasForeignKey(k => k.KullaniciId);

            base.Configure(builder);
        }
    }
}
