using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ParaYolYonetici.Core.Interfaces;

namespace ParaYolYonetici.Api.ActionFilters.PermissionControl
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IKullaniciYetkileriRepository _kullaniciYetkileriRepository;

        public PermissionHandler(IKullaniciYetkileriRepository kullaniciYetkileriRepository)
        {
            _kullaniciYetkileriRepository = kullaniciYetkileriRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(x => x.Type == "nameidentifier")?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                context.Fail();
                return;
            }

            //bool hasAnyPermission = await _dbContext.KullaniciYetkileri
            //    .AnyAsync(y =>
            //        y.KullaniciId == userId &&
            //        requirement.YetkiIds.Contains(y.YetkiId) && // Contains ile çoklu kontrol
            //        !y.SilindiMi);

            bool hasAnyPermission = await _kullaniciYetkileriRepository.KullaniciYetkiVarMi(userId, requirement.YetkiIds);

            if (hasAnyPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
