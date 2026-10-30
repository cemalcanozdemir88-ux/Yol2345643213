using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Configurations
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
