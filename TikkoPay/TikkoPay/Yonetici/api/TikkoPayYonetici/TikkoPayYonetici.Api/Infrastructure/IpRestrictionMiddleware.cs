using TikkoPayYonetici.Core.Interfaces;

namespace TikkoPayYonetici.Api.Infrastructure
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;

        public IpRestrictionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IKullanicilarRepository kullanicilarRepository,
            IKullaniciIpAdresleriRepository kullaniciIpAdresleriRepository)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return; 
            }

            var userId = context.User.Claims.FirstOrDefault(c => c.Type == "nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Yetkisiz erişim.");
                return;
            }

            _ = Guid.TryParse(userId, out var _userId);

            var user = await kullanicilarRepository.GetByIdAsync(_userId);
            if (user == null || !user.Durum || user.SilindiMi)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is inactive");
                return;
            }

            if (!user.IpLoggingEnabled)
            {
                await _next(context);
                return;
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            var kullaniciIpAdresList = await kullaniciIpAdresleriRepository.GetByKullaniciIdAsync(user.Id);
            var allowedIps = kullaniciIpAdresList.Select(ip => ip.IpAdresi).ToList();
            if (!allowedIps.Contains(clientIp))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Geçersiz IP adresi.");
                return;
            }

            await _next(context);
        }
    }
}
