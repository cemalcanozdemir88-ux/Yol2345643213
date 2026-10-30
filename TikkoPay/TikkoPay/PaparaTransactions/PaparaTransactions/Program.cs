using Serilog;
using Transactions.Helpers;
using Transactions.Infrastructure;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Concretes;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddScoped<IWebSiteRepository, WebSiteRepository>();
            builder.Services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            builder.Services.AddScoped<IDepositRepository, DepositRepository>();
            builder.Services.AddScoped<IYatirimCekimTalepleriRepository, YatirimCekimTalepleriRepository>();
            builder.Services.AddScoped<IHavaleBankalariRepository, HavaleBankalariRepository>();
            builder.Services.AddScoped<ITedarikcilerRepository, TedarikcilerRepository>();
            builder.Services.AddScoped<IGeneralHelper, GeneralHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSerilogRequestLogging();

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
