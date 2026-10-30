using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaparaYonetici.Infrastructure
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
            services.AddScoped<IWebSiteleriRepository, WebSiteleriRepository>();
            services.AddScoped<ITedarikciRepository, TedarikciRepository>();
            services.AddScoped<IKullanicilarRepository, KullanicilarRepository>();
            services.AddScoped<IKullaniciYetkileriRepository, KullaniciYetkileriRepository>();
            services.AddScoped<IBankalarRepository, BankalarRepository>();
            services.AddScoped<IHavaleBankalariRepository, HavaleBankalariRepository>();
            services.AddScoped<IYatirimTalepleriRepository, YatirimTalepleriRepository>();
            services.AddScoped<IKasaHaraketleriRepository, KasaHaraketleriRepository>();
            services.AddScoped<IWebSiteKasaHaraketleriRepository, WebSiteKasaHaraketleriRepository>();
            services.AddScoped<IDepositTransactionRepository, DepositTransactionRepository>();
            services.AddScoped<IWithdrawTransactionRepository, WithdrawTransactionRepository>();
            services.AddScoped<IAffiliateRepository, AffiliateRepository>();
            services.AddScoped<IKullaniciOtpValuesRepository, KullaniciOtpValuesRepository>();

            return services;
        }
    }
}
