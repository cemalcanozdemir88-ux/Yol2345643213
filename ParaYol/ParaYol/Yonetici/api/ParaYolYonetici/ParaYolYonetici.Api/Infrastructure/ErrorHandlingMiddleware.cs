using System.Text;
using System.Text.Json;
using Serilog;

namespace ParaYolYonetici.Api.Infrastructure
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
                var uniqueLogFileName = $"logs/error/log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt";
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(uniqueLogFileName, encoding: Encoding.UTF8)
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
                message = exception.Message,
                detail = exception.InnerException?.Message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
