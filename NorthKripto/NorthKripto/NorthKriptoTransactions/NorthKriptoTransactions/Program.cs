using System.Globalization;
using NorthKriptoTransactions.Helpers;
using NorthKriptoTransactions.Infrastructure;
using NorthKriptoTransactions.Infrastructure.Helpers;
using NorthKriptoTransactions.Infrastructure.Repositories.Concretes;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;
using NorthKriptoTransactions.Infrastructure.Services.Concretes;
using NorthKriptoTransactions.Infrastructure.Services.Interfaces;
using Serilog;

namespace NorthKriptoTransactions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cultureInfo = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddScoped<IGeneralHelper, GeneralHelper>();

            #region Repositories Scope

            builder.Services.AddScoped<ICekimTalepleriRepository, CekimTalepleriRepository>();
            builder.Services.AddScoped<IDepositRepository, DepositRepository>();
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
            builder.Services.AddScoped<IWebSitePaymentMethodsRepository, WebSitePaymentMethodsRepository>();
            builder.Services.AddScoped<IWebSiteRepository, WebSiteRepository>();
            builder.Services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            builder.Services.AddScoped<IYatirimTalepleriRepository, YatirimTalepleriRepository>();
            

            #endregion Repositories Scope

            #region Services Scope

            builder.Services.AddScoped<ITronService, TronService>();
            builder.Services.AddScoped<IHDWalletService, HDWalletService>();

            #endregion Services Scope

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/robots.txt"))
                {
                    var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, "robots.txt");
                    string output = "User-agent: *\nDisallow: /";
                    if (File.Exists(robotsTxtPath))
                    {
                        output = await File.ReadAllTextAsync(robotsTxtPath);
                    }
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(output);
                }
                else
                {
                    await next();
                }
            });

            app.Use(async (context, next) =>
            {
                var userAgent = context.Request.Headers.UserAgent.ToString();
                if (userAgent.Contains("google-read-aloud", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Google-Read-Aloud bot istekleri engellendi.");
                    return;
                }

                await next();
            });

            app.UseSerilogRequestLogging();

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
