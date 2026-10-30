using ParaYolYonetici.Services.Concretes;
using ParaYolYonetici.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ParaYolYonetici.Services
{
    public static class DIRegistration
    {
        public static IServiceCollection ConfigureServiceLayerDiRegistration(this IServiceCollection services)
        {
            services.AddScoped<IWebSiteleriService, WebSiteleriService>();
            services.AddScoped<ITedarikciService, TedarikciService>();
            services.AddScoped<IKullanicilarService, KullanicilarService>();
            services.AddScoped<IKullaniciYetkileriService, KullaniciYetkileriService>();
            services.AddScoped<IBankaService, BankaService>();
            services.AddScoped<IHavaleBankalariService, HavaleBankalariService>();
            services.AddScoped<IYatirimTalepleriService, YatirimTalepleriService>();
            services.AddScoped<ICekimTalepleriService, CekimTalepleriService>();
            services.AddScoped<IKasaHaraketleriService, KasaHaraketleriService>();
            services.AddScoped<IWebSiteKasaHaraketleriService, WebSiteKasaHaraketleriService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<IKullaniciBlackListsService, KullaniciBlackListsService>();
            services.AddScoped<IKullaniciIpAdresleriService, KullaniciIpAdresleriService>();
            services.AddScoped<ITelegramBotService, TelegramBotService>();
            services.AddScoped<IWebSiteTedarikcileriService, WebSiteTedarikcileriService>();
            services.AddScoped<IWebSiteTedarikcileriManualService, WebSiteTedarikcileriManualService>();
            services.AddScoped<IYatirimTalepleriManuelService, YatirimTalepleriManuelService>();
            services.AddScoped<IViPayService, ViPayService>();
            services.AddScoped<ITedarikciMutabakatService, TedarikciMutabakatService>();
            services.AddScoped<IWebSiteMutabakatService, WebSiteMutabakatService>();
            services.AddScoped<IWebSitePaymentMethodService, WebSitePaymentMethodService>();
            services.AddScoped<IPaymentMethodsService, PaymentMethodsService>();
            services.AddScoped<INhService, NhService>();

            return services;
        }
    }
}
