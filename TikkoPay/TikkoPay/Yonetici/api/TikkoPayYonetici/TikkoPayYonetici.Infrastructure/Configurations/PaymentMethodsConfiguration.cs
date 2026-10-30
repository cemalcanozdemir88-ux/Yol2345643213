using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Configurations
{
    public class PaymentMethodsConfiguration : IEntityTypeConfiguration<PaymentMethods>
    {
        public void Configure(EntityTypeBuilder<PaymentMethods> builder)
        {
            builder.ToTable("PaymentMethods");

            builder.Property(k => k.Baslik).IsRequired(true).HasMaxLength(250);
            builder.Property(k => k.PaymentId).IsRequired(true);
            builder.Property(k => k.DepositCallbackUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.WithdrawCallbackUrl).IsRequired(false).HasMaxLength(250);

        }
    }
}
