using TikkoPayYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TikkoPayYonetici.Infrastructure.Configurations
{
    public class WebSiteHavaleBankalariConfiguration : BaseModelConfiguration<WebSiteHavaleBankalari>, IEntityTypeConfiguration<WebSiteHavaleBankalari>
    {
        public override void Configure(EntityTypeBuilder<WebSiteHavaleBankalari> builder)
        {
            builder.ToTable("WebSiteHavaleBankalari");

            builder.HasOne(k => k.WebSiteleri)
                   .WithMany()
                   .HasForeignKey(k => k.WebSiteId);

            builder.HasOne(k => k.HavaleBankalari)
                   .WithMany()
                   .HasForeignKey(k => k.HavaleBankaId);

            base.Configure(builder);
        }
    }
}
