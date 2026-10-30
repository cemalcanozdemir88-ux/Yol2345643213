using PaparaYonetici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaYonetici.Infrastructure.Configurations
{
    public class YetkilerConfiguration : BaseModelConfiguration<Yetkiler>, IEntityTypeConfiguration<Yetkiler>
    {
        public override void Configure(EntityTypeBuilder<Yetkiler> builder)
        {
            builder.ToTable("Yetkiler");

            builder.Property(k => k.Baslik).IsRequired().HasMaxLength(500);

            base.Configure(builder);
        }
    }
}
