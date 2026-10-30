using TikkoPayYonetici.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using TikkoPayYonetici.Api.ActionFilters;

namespace TikkoPayYonetici.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected KullaniciTip GetUserRole()
        {
            var userRole = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .FirstOrDefault();

            if (userRole == null)
            {
                throw new Exception("user role bulunamadı");
            }

            Enum.TryParse(userRole, out KullaniciTip kullaniciTip);
            return kullaniciTip;
        }

        protected bool IsSuperAdmin()
        {
            var userRole = GetUserRole();
            return userRole == KullaniciTip.Admin;
        }

        protected bool IsWebSiteUser()
        {
            var userRole = GetUserRole();
            return userRole == KullaniciTip.WebSiteKullanicisi;
        }

        protected Guid LoggedInUserId
        {
            get
            {
                var id = User.Claims
                   .Where(c => c.Type == "nameidentifier")
                   .Select(c => c.Value)
                   .FirstOrDefault();

                return Guid.Parse(id);
            }
        }

        protected Guid GetTedarikciId()
        {
            var id = User.Claims
                   .Where(c => c.Type == "tedarikciId")
                   .Select(c => c.Value)
                   .FirstOrDefault();

            return Guid.Parse(id);
        }

        protected Guid GetWebSiteId()
        {
            var id = User.Claims
                   .Where(c => c.Type == "websiteId")
                   .Select(c => c.Value)
                   .FirstOrDefault();

            return Guid.Parse(id);
        }

        protected Guid GetAffiliateId()
        {
            var id = User.Claims
                   .Where(c => c.Type == "affiliateId")
                   .Select(c => c.Value)
                   .FirstOrDefault();

            return Guid.Parse(id);
        }

        protected DateTime? StringToDateTime(string? datetime)
        {
            DateTime? outputDate = null;
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                outputDate = DateTime.ParseExact(datetime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return outputDate;
        }
    }
}
