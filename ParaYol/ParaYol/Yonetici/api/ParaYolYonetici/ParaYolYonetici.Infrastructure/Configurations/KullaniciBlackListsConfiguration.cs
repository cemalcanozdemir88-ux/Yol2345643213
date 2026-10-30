using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Infrastructure.Configurations
{
    public class KullaniciBlackListsConfiguration : BaseModelConfiguration<KullaniciBlackLists>, IEntityTypeConfiguration<KullaniciBlackLists>
    {
        public override void Configure(EntityTypeBuilder<KullaniciBlackLists> builder)
        {
            builder.ToTable("KullaniciBlackLists");

            builder.HasOne(k => k.WebSiteleri)
                   .WithMany(t => t.KullaniciBlackLists)
                   .HasForeignKey(k => k.WebSiteId);

            base.Configure(builder);
        }
    }
}
