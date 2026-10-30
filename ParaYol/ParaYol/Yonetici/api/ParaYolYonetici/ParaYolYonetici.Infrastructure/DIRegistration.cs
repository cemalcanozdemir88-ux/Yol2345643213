using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ParaYolYonetici.Infrastructure
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
            services.AddScoped<IYatirimTalepleriManualRepository, YatirimTalepleriManualRepository>();
            services.AddScoped<ICekimTalepleriRepository, CekimTalepleriRepository>();
            services.AddScoped<IKasaHaraketleriRepository, KasaHaraketleriRepository>();
            services.AddScoped<IWebSiteKasaHaraketleriRepository, WebSiteKasaHaraketleriRepository>();
            services.AddScoped<IDepositTransactionRepository, DepositTransactionRepository>();
            services.AddScoped<IWithdrawTransactionRepository, WithdrawTransactionRepository>();
            services.AddScoped<IAffiliateRepository, AffiliateRepository>();
            services.AddScoped<IKullaniciOtpValuesRepository, KullaniciOtpValuesRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IKullaniciBlackListsRepository, KullaniciBlackListsRepository>();
            services.AddScoped<IActionRequestLogRepository, ActionRequestLogRepository>();
            services.AddScoped<IKullaniciLoginRepository, KullaniciLoginRepository>();
            services.AddScoped<IKullaniciIpAdresleriRepository, KullaniciIpAdresleriRepository>();
            services.AddScoped<IWebSitePaymentMethodsRepository, WebSitePaymentMethodsRepository>();
            services.AddScoped<IWebSiteTedarikcileriRepository, WebSiteTedarikcileriRepository>();
            services.AddScoped<IWebSiteTedarikcileriManualRepository, WebSiteTedarikcileriManualRepository>();
            services.AddScoped<ITedarikciMutabakatRepository, TedarikciMutabakatRepository>();
            services.AddScoped<IWebSiteMutabakatRepository, WebSiteMutabakatRepository>();
            services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();

            return services;
        }
    }
}
