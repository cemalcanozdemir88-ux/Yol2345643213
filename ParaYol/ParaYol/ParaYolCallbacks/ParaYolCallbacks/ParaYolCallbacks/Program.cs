
using ParaYolCallbacks.Infrastructure;
using ParaYolCallbacks.Infrastructure.Helpers;
using ParaYolCallbacks.Infrastructure.Repositories.Concretes;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;
using ParaYolCallbacks.Infrastructure.Services.Concretes;
using ParaYolCallbacks.Infrastructure.Services.Interfaces;
using Serilog;

namespace ParaYolCallbacks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
            builder.Services.AddScoped<IWebSiteRepository, WebSiteRepository>();
            builder.Services.AddScoped<IDepositRepository, DepositRepository>();
            builder.Services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            builder.Services.AddScoped<ITedarikcilerRepository, TedarikcilerRepository>();
            builder.Services.AddScoped<IYatirimCekimTalepleriRepository, YatirimCekimTalepleriRepository>();
            builder.Services.AddScoped<IWebSitePaymentMethodsRepository, WebSitePaymentMethodsRepository>();

            builder.Services.AddScoped<IViPayDepositCallbackService, ViPayDepositCallbackService>();
            builder.Services.AddScoped<IViPayWithdrawCallbackService, ViPayWithdrawCallbackService>();

            builder.Services.AddScoped<INhDepositCallbackService, NhDepositCallbackService>();
            builder.Services.AddScoped<INhWithdrawCallbackService, NhWithdrawCallbackService>();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
