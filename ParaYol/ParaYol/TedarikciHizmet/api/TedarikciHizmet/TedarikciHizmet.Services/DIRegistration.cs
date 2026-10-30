using TedarikciHizmet.Services.Concretes;
using TedarikciHizmet.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TedarikciHizmet.Services
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
            services.AddScoped<IYatirimTalepleriManualService, YatirimTalepleriManualService>();
            services.AddScoped<ICekimTalepleriService, CekimTalepleriService>();
            services.AddScoped<IKasaHaraketleriService, KasaHaraketleriService>();
            services.AddScoped<IWebSiteKasaHaraketleriService, WebSiteKasaHaraketleriService>();
            services.AddScoped<IImgbbUploadService, ImgbbUploadService>();
            services.AddScoped<IImageUploadService, ImageUploadService>();
            services.AddScoped<ITelegramBotService, TelegramBotService>();
            services.AddScoped<ITedarikciMutabakatService, TedarikciMutabakatService>();

            return services;
        }
    }
}
