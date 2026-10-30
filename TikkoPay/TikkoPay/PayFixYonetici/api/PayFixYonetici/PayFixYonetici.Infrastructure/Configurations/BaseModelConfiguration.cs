using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Infrastructure.Configurations
{
    public class BaseModelConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.KayitTarihi).HasColumnType("datetime").IsRequired();
            builder.Property(k => k.KaydedenKullaniciId).IsRequired(false);
            builder.Property(k => k.GuncellemeTarihi).HasColumnType("datetime");
            builder.Property(k => k.GuncelleyenKullaniciId).IsRequired(false);
        }
    }
}
