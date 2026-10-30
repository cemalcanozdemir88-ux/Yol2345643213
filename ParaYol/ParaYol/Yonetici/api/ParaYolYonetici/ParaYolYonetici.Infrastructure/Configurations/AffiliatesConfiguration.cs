using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Configurations
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
