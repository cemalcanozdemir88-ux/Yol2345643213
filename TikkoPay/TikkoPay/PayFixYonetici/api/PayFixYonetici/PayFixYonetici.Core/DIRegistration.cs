using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PayFixYonetici.Core
{
    public static class DIRegistration
    {
        public static IServiceCollection ConfigureCoreLayerDiRegistration(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
