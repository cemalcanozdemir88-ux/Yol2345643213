using Microsoft.AspNetCore.Authorization;

namespace ParaYolYonetici.Api.ActionFilters.PermissionControl
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public List<Guid> YetkiIds { get; }

        public PermissionRequirement(IEnumerable<Guid> yetkiIds)
        {
            YetkiIds = yetkiIds.ToList();
        }
    }
}
