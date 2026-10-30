using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Configurations
{
    public class WebSitePaymentMethodsConfiguration : BaseModelConfiguration<WebSitePaymentMethods>, IEntityTypeConfiguration<WebSitePaymentMethods>
    {
        public override void Configure(EntityTypeBuilder<WebSitePaymentMethods> builder)
        {
            builder.ToTable("WebSitePaymentMethods");

            builder.Property(k => k.WebSiteleriId).IsRequired(true);
            builder.Property(k => k.PaymentMethodId).IsRequired(true);
            builder.Property(k => k.ApiClientId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.ApiClientSecret).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.DepositCallbackUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.WithdrawCallbackUrl).IsRequired(false).HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
