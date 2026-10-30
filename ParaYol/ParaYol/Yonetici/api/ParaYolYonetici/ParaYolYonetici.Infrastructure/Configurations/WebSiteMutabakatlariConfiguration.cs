using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Configurations
{
    public class WebSiteMutabakatlariConfiguration : BaseModelConfiguration<WebSiteMutabakatlari>, IEntityTypeConfiguration<WebSiteMutabakatlari>
    {
        public override void Configure(EntityTypeBuilder<WebSiteMutabakatlari> builder)
        {
            builder.ToTable("WebSiteMutabakatlari");

            builder.HasOne(k => k.WebSiteleri)
              .WithMany(t => t.WebSiteMutabakatlariList)
              .HasForeignKey(k => k.WebSiteId);

            base.Configure(builder);
        }
    }
}
