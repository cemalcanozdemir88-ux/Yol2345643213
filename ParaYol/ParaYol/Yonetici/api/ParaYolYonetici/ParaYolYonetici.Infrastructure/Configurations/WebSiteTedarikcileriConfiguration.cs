using ParaYolYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ParaYolYonetici.Infrastructure.Configurations
{
    public class WebSiteTedarikcileriConfiguration : BaseModelConfiguration<WebSiteTedarikcileri>, IEntityTypeConfiguration<WebSiteTedarikcileri>
    {
        public override void Configure(EntityTypeBuilder<WebSiteTedarikcileri> builder)
        {
            builder.ToTable("WebSiteTedarikcileri");

            builder.HasOne(k => k.WebSiteleri)
                   .WithMany(t => t.WebSiteTedarikcileriList)
                   .HasForeignKey(k => k.WebSiteId);

            builder.HasOne(k => k.Tedarikciler)
                   .WithMany(t => t.WebSiteTedarikcileriList)
                   .HasForeignKey(k => k.TedarikciId);

            base.Configure(builder);
        }
    }
}
