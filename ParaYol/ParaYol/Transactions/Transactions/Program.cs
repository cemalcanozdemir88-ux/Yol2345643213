using System.Globalization;
using System.Threading.RateLimiting;
using Serilog;
using Transactions.Helpers;
using Transactions.Infrastructure;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Concretes;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Concretes;
using Transactions.Infrastructure.Services.Interfaces;

namespace Transactions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100, // 1 dakikada max 100 istek
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 10
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            var cultureInfo = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

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
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
            builder.Services.AddScoped<IKullaniciBlackListsRepository, KullaniciBlackListsRepository>();
            builder.Services.AddScoped<IGeneralHelper, GeneralHelper>();
            builder.Services.AddScoped<IWebSitePaymentMethodsRepository, WebSitePaymentMethodsRepository>();
            builder.Services.AddScoped<IBankaKodlariRepository, BankaKodlariRepository>();

            builder.Services.AddScoped<IViPayService, ViPayService>();
            builder.Services.AddScoped<INhService, NhService>();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRateLimiter();

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
