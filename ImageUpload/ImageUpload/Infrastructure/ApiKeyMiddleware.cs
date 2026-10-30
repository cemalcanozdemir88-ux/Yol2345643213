namespace ImageUpload.Infrastructure
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var path = context.Request.Path;
            if (!path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key eksik.");
                return;
            }

            var configuredApiKey = configuration["ApiKeySettings:Key"];
            if (!configuredApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Geçersiz API Key.");
                return;
            }

            await _next(context);
        }
    }
}
