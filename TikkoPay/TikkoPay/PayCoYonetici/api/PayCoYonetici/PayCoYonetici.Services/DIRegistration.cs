using Microsoft.Extensions.DependencyInjection;
using PayCoYonetici.Services.Concretes;
using PayCoYonetici.Services.Interfaces;

namespace PayCoYonetici.Services
{
    public static class DIRegistration
    {
        public static IServiceCollection ConfigureServiceLayerDiRegistration(this IServiceCollection services)
        {
            services.AddScoped<IKullanicilarService, KullanicilarService>();
            services.AddScoped<IKullaniciYetkileriService, KullaniciYetkileriService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<IWebSiteleriService, WebSiteleriService>();
            services.AddScoped<IWebSiteKasaHaraketleriService, WebSiteKasaHaraketleriService>();
            services.AddScoped<ICekimTalepleriService, CekimTalepleriService>();
            services.AddScoped<IYatirimTalepleriService, YatirimTalepleriService>();
            services.AddScoped<IKasaHaraketleriService, KasaHaraketleriService>();

            return services;
        }
    }
}
