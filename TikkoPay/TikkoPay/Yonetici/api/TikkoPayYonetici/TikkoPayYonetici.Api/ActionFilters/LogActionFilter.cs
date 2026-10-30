using Microsoft.AspNetCore.Mvc.Filters;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Api.ActionFilters
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly IActionRequestLogRepository _actionRequestLogRepository;
        private readonly IKullanicilarRepository _kullanicilarRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogActionFilter(IActionRequestLogRepository actionRequestLogRepository,
                               IKullanicilarRepository kullanicilarRepository,
                               IUnitOfWork unitOfWork)
        {
            _actionRequestLogRepository = actionRequestLogRepository;
            _kullanicilarRepository = kullanicilarRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

            var nameIdentifier = httpContext.User.FindFirst(x => x.Type == "nameidentifier")?.Value;
            Guid? kullaniciId = nameIdentifier != null ? Guid.Parse(nameIdentifier) : null;

            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];

            var kullaniciItem = await _kullanicilarRepository.GetByIdAsync(kullaniciId.Value);

            await _actionRequestLogRepository.AddAsync(new ActionRequestLog
            {
                Id = Guid.NewGuid(),
                KullaniciId = kullaniciId,
                Controller = controller.ToString(),
                Action = action.ToString(),
                IpAddress = kullaniciItem.IpLoggingEnabled ? ipAddress : string.Empty,
            });
            await _unitOfWork.SaveAsync();

            await next();
        }
    }
}
