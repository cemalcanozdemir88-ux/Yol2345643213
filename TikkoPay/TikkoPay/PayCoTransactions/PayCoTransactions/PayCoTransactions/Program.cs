using PayCoTransactions.Infrastructure;
using PayCoTransactions.Infrastructure.Helpers;
using PayCoTransactions.Infrastructure.Middlewares;
using PayCoTransactions.Infrastructure.Repositories.Concretes;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Infrastructure.Services.Concretes;
using PayCoTransactions.Infrastructure.Services.Interfaces;
using Serilog;

namespace PayCoTransactions
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
