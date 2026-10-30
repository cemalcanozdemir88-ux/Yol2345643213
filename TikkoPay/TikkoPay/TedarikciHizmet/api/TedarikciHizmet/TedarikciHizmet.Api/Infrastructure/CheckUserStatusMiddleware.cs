using TedarikciHizmet.Core.Interfaces;

namespace TedarikciHizmet.Api.Infrastructure
{
    public class CheckUserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckUserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IKullanicilarRepository _kullanicilarRepository)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "nameidentifier")?.Value;
                if (userId != null)
                {
                    _ = Guid.TryParse(userId, out var _userId);
                    var user = await _kullanicilarRepository.GetByIdAsync(_userId);
                    if (user == null || !user.Durum || user.SilindiMi)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("User is inactive");

                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
