using PaparaTedarikciHizmet.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace PaparaTedarikciHizmet.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected KullaniciTip GetUserRole()
        {
            var userRole = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .FirstOrDefault() ?? throw new Exception("user role bulunamadı");

            _ = Enum.TryParse(userRole, out KullaniciTip kullaniciTip);
            return kullaniciTip;
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

        protected DateTime? StringToDateTime(string datetime)
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
