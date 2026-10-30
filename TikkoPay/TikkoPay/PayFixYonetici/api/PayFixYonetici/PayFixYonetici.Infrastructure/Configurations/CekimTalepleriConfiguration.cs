using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Configurations
{
    public class CekimTalepleriConfiguration : BaseModelConfiguration<CekimTalepleri>, IEntityTypeConfiguration<CekimTalepleri>
    {
        public override void Configure(EntityTypeBuilder<CekimTalepleri> builder)
        {
            builder.ToTable("CekimTalepleri");

            builder.Property(k => k.IdNumber).ValueGeneratedOnAdd().UseIdentityColumn(1000, 1);

            builder.Property(k => k.TalepNo).IsRequired().HasMaxLength(250);
            builder.Property(k => k.WebSiteTransactionId).IsRequired(false).HasMaxLength(250);
            builder.Property(k => k.AdSoyad).IsRequired().HasMaxLength(250);
            builder.Property(k => k.UserName).HasMaxLength(250);
            builder.Property(k => k.UserId).HasMaxLength(250);
            builder.Property(k => k.Tutar).HasColumnType("decimal(13,4)");
            builder.Property(k => k.WebSiteKomisyon).HasColumnType("decimal(13,4)");
            builder.Property(k => k.Toplam).HasColumnType("decimal(13,4)");
            builder.Property(k => k.IsSendToWebSite).IsRequired(true).HasDefaultValue(false);
            builder.Property(k => k.WebSiteCallBackResult).IsRequired(false);
            builder.Property(k => k.IptalAciklama).HasMaxLength(1500);

            builder.HasOne(k => k.WebSiteleri)
                   .WithMany(t => t.CekimTalepleri)
                   .HasForeignKey(k => k.WebSiteId);

            base.Configure(builder);
        }
    }
}
