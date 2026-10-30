using PaparaYonetici.Services.Concretes;
using PaparaYonetici.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PaparaYonetici.Services
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

            return services;
        }
    }
}
