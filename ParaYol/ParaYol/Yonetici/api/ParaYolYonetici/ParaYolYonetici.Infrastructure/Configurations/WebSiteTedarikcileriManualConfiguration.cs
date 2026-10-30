using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Configurations
{
    public class WebSiteTedarikcileriManualConfiguration : BaseModelConfiguration<WebSiteTedarikcileriManual>, IEntityTypeConfiguration<WebSiteTedarikcileriManual>
    {
        public override void Configure(EntityTypeBuilder<WebSiteTedarikcileriManual> builder)
        {
            builder.ToTable("WebSiteTedarikcileriManual");

            builder.HasOne(k => k.WebSiteleri)
                   .WithMany(t => t.WebSiteTedarikcileriManualList)
                   .HasForeignKey(k => k.WebSiteId);

            builder.HasOne(k => k.Tedarikciler)
                   .WithMany(t => t.WebSiteTedarikcileriManualList)
                   .HasForeignKey(k => k.TedarikciId);

            base.Configure(builder);
        }
    }
}
