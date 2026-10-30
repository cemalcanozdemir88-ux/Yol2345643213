using PayFixTransactions.Infrastructure;
using PayFixTransactions.Infrastructure.Helpers;
using PayFixTransactions.Infrastructure.Middlewares;
using PayFixTransactions.Infrastructure.Repositories.Concretes;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Infrastructure.Services.Concretes;
using PayFixTransactions.Infrastructure.Services.Interfaces;
using Serilog;

namespace PayFixTransactions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<DapperContext>();

            builder.Services.AddScoped<IGeneralHelper, GeneralHelper>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IDepositTransactionRepository, DepositTransactionRepository>();
            builder.Services.AddScoped<IWithdrawTransactionRepository, WithdrawTransactionRepository>();
            builder.Services.AddScoped<IWebSiteRepository, WebSiteRepository>();
            builder.Services.AddScoped<ICallbackRepository, CallbackRepository>();
            builder.Services.AddScoped<IYatirimTalepleriRepository, YatirimTalepleriRepository>();
            builder.Services.AddScoped<ICekimTalepleriRepository, CekimTalepleriRepository>();

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
