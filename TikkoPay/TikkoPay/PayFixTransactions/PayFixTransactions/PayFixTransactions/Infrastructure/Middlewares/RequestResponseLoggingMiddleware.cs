using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace PayFixTransactions.Infrastructure.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path.ToString().ToLower();
            var excludedExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".woff", ".woff2", ".ttf", ".eot", ".ico" };

            // Eğer istek statik dosya ise loglama yapma
            if (excludedExtensions.Any(ext => requestPath.EndsWith(ext)))
            {
                await _next(context);
                return;
            }

            var logFilePath = $"logs/request/{DateTime.Now:yyyy-MM-dd}/log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt";

            // İstek loglama
            var request = await FormatRequest(context.Request);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, encoding: Encoding.UTF8)
                .CreateLogger();
            Log.Information("{Request}", request);

            // Yanıt loglama
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                var response = await FormatResponse(context.Response);
                Log.Information("{Response}", response);

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"Headers: {JsonConvert.SerializeObject(request.Headers, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            stringBuilder.AppendLine($"Method: {JsonConvert.SerializeObject(request.Method, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            stringBuilder.AppendLine($"Path: {JsonConvert.SerializeObject(request.Path, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            stringBuilder.AppendLine($"Query: {JsonConvert.SerializeObject(request.Query, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            stringBuilder.AppendLine($"RouteValues: {JsonConvert.SerializeObject(request.RouteValues, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            stringBuilder.AppendLine($"Body: {bodyAsText}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("************************************");
            stringBuilder.AppendLine("             EOF REQUEST            ");
            stringBuilder.AppendLine("************************************");
            stringBuilder.AppendLine("");

            return stringBuilder.ToString();

        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body, Encoding.UTF8).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Status Code: {response.StatusCode}, Body: {text}";
        }

    }
}
