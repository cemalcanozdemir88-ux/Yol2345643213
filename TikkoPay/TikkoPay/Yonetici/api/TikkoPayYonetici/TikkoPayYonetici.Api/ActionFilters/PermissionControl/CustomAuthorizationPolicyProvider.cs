using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TikkoPayYonetici.Api.ActionFilters.PermissionControl
{
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private const string Separator = ",";

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PermissionAuthorizeAttribute.PolicyPrefix))
            {
                // Policy adından yetki ID'lerini çıkar
                var yetkiIdList = policyName
                    .Substring(PermissionAuthorizeAttribute.PolicyPrefix.Length)
                    .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr => Guid.Parse(idStr))
                    .ToList();

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(yetkiIdList));
                return policy.Build();
            }

            return await base.GetPolicyAsync(policyName);
        }
    }
}
