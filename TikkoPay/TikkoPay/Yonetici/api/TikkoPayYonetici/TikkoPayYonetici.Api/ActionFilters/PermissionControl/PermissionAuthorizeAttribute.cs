using Microsoft.AspNetCore.Authorization;

namespace TikkoPayYonetici.Api.ActionFilters.PermissionControl
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "Yetki_";
        private const string Separator = ",";

        public PermissionAuthorizeAttribute(params string[] yetkiIds)
        {
            // Yetki ID'leri virgülle birleştirerek policy adı oluştur
            Policy = $"{PolicyPrefix}{string.Join(Separator, yetkiIds)}";
        }
    }
}
