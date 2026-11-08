using TikkoPayYonetici.Services.Concretes;
using TikkoPayYonetici.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TikkoPayYonetici.Services
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
            services.AddScoped<IFluxFinansService, FluxFinansService>();
            services.AddScoped<ITelegramBotService, TelegramBotService>();
            services.AddScoped<IWebSiteTedarikcileriService, WebSiteTedarikcileriService>();
            services.AddScoped<IWebSiteHavaleBankalariService, WebSiteHavaleBankalariService>();
            services.AddScoped<IWebSiteTedarikcileriManualService, WebSiteTedarikcileriManualService>();
            services.AddScoped<IYatirimTalepleriManuelService, YatirimTalepleriManuelService>();
            services.AddScoped<IViPayService, ViPayService>();
            services.AddScoped<IPullPayService, PullPayService>();
            services.AddScoped<IParoPayService, ParoPayService>();
            services.AddScoped<IZenService, ZenService>();
            services.AddScoped<ITumBankService, TumBankService>();
            services.AddScoped<ITedarikciMutabakatService, TedarikciMutabakatService>();
            services.AddScoped<IWebSiteMutabakatService, WebSiteMutabakatService>();
            services.AddScoped<IWebSitePaymentMethodService, WebSitePaymentMethodService>();
            services.AddScoped<IPaymentMethodsService, PaymentMethodsService>();
            services.AddScoped<IFixHavaleService, FixHavaleService>();

            return services;
        }
    }
}
