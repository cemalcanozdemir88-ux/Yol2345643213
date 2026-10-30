using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
{
    public class WithdrawTransactionConfiguration : IEntityTypeConfiguration<WithdrawTransaction>
    {
        public void Configure(EntityTypeBuilder<WithdrawTransaction> builder)
        {
            builder.ToTable("WithdrawTransactions");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.WebSiteId).IsRequired();
            builder.Property(k => k.UserName).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.UserId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.FullName).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.TransactionId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.Amount).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.Iban).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.ClientId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.ClientSecret).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.Hash).IsRequired(false).HasMaxLength(1500);
            builder.Property(k => k.KayitTarihi).HasColumnType("datetime").IsRequired();
        }
    }
}
