using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Configurations
{
    public class WebSiteleriConfiguration : BaseModelConfiguration<WebSiteleri>, IEntityTypeConfiguration<WebSiteleri>
    {
        public override void Configure(EntityTypeBuilder<WebSiteleri> builder)
        {
            builder.ToTable("WebSiteleri");

            builder.Property(k => k.WebSiteUrl).IsRequired().HasMaxLength(250);
            builder.Property(k => k.ApiClientId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.ApiClientSecret).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.DepositCallbackUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.WithdrawCallbackUrl).IsRequired(false).HasMaxLength(250);

            builder.HasOne(k => k.Affiliate)
                   .WithMany(t => t.WebSiteleri)
                   .HasForeignKey(k => k.AffiliateId);

            base.Configure(builder);
        }
    }
}
