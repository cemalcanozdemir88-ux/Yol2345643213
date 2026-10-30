using PaparaTedarikciHizmet.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaparaTedarikciHizmet.Infrastructure.Configurations
{
    public class BankalarConfiguration : BaseModelConfiguration<Bankalar>, IEntityTypeConfiguration<Bankalar>
    {
        public override void Configure(EntityTypeBuilder<Bankalar> builder)
        {
            builder.ToTable("Bankalar");

            builder.Property(k => k.Baslik).IsRequired().HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
