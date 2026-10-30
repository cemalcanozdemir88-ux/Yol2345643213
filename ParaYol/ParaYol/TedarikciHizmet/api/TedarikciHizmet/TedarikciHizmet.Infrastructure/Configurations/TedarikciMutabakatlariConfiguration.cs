using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Infrastructure.Configurations
{
    public class TedarikciMutabakatlariConfiguration : BaseModelConfiguration<TedarikciMutabakatlari>, IEntityTypeConfiguration<TedarikciMutabakatlari>
    {
        public override void Configure(EntityTypeBuilder<TedarikciMutabakatlari> builder)
        {
            builder.ToTable("TedarikciMutabakatlari");

            builder.HasOne(k => k.Tedarikciler)
              .WithMany(t => t.TedarikciMutabakatlari)
              .HasForeignKey(k => k.TedarikciId);

            base.Configure(builder);
        }
    }
}
