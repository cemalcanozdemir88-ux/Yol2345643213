using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
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

            base.Configure(builder);
        }
    }
}
