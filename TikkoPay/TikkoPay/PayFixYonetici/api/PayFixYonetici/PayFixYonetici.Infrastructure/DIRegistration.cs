using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayFixYonetici.Core.Interfaces;
using PayFixYonetici.Infrastructure.Repositories;

namespace PayFixYonetici.Infrastructure
{
    public static class DIRegistration
    {
        public static IServiceCollection ConfigureInfrastructureLayerDiRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddHttpContextAccessor();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IKullanicilarRepository, KullanicilarRepository>();
            services.AddScoped<IKullaniciOtpValuesRepository, KullaniciOtpValuesRepository>();
            services.AddScoped<IKullaniciYetkileriRepository, KullaniciYetkileriRepository>();
            services.AddScoped<IAffiliateRepository, AffiliateRepository>();
            services.AddScoped<IWebSiteleriRepository, WebSiteleriRepository>();
            services.AddScoped<IWebSiteKasaHaraketleriRepository, WebSiteKasaHaraketleriRepository>();
            services.AddScoped<ICekimTalepleriRepository, CekimTalepleriRepository>();
            services.AddScoped<IYatirimTalepleriRepository, YatirimTalepleriRepository>();
            services.AddScoped<IKasaHaraketleriRepository, KasaHaraketleriRepository>();

            return services;
        }
    }
}
