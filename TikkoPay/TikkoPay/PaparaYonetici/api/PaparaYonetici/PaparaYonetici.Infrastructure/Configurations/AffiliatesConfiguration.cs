using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Infrastructure.Configurations
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
