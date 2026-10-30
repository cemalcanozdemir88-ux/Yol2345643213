using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Configurations
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

            builder.HasOne(k => k.PaymentMethods)
                   .WithMany(t => t.WebSitePaymentMethods)
                   .HasForeignKey(k => k.PaymentMethodId)
                   .HasPrincipalKey(t => t.Id);

            base.Configure(builder);
        }
    }
}
