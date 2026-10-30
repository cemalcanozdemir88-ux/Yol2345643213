using Serilog;
using System.Text.Json;

namespace Transactions.Helpers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var uniqueLogFileName = $"logs/error/log-{DateTime.Now:yyyyMMddHHmmssfff}.txt";
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(uniqueLogFileName)
                    .CreateLogger();

                Log.Error(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                context.Response.StatusCode,
                Message = "Internal Server Error"
            });

            return context.Response.WriteAsync(result);
        }
    }
}
