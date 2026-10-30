using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Configurations
{
    public class AffiliatesConfiguration : BaseModelConfiguration<Affiliates>, IEntityTypeConfiguration<Affiliates>
    {
        public override void Configure(EntityTypeBuilder<Affiliates> builder)
        {
            builder.ToTable("Affiliates");
            builder.Property(k => k.Unvan).IsRequired().HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
