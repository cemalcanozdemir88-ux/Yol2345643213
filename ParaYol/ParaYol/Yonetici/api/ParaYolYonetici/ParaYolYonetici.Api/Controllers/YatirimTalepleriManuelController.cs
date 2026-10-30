using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Api.ActionFilters.PermissionControl;
using ParaYolYonetici.Api.Infrastructure;
using ParaYolYonetici.Core.DTOs.YatirimTalepleri;
using ParaYolYonetici.Services.Concretes;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class YatirimTalepleriManuelController : BaseController
    {
        private readonly IYatirimTalepleriManuelService _manuelYatirimTalepleriService;

        public YatirimTalepleriManuelController(IYatirimTalepleriManuelService manuelYatirimTalepleriService)
        {
            _manuelYatirimTalepleriService = manuelYatirimTalepleriService;
        }

        [PermissionAuthorize(Yetki.IptalEdilenManualYatirimlarBekleyeneAlabilir)]
        [HttpPut("bekleyene-al")]
        public async Task<IActionResult> BekleyeneAl(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _manuelYatirimTalepleriService.BekleyeneAl(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.ManualYatirimlarBekleyenGorebilir)]
        [HttpGet("bekleyen-yatirim-talepleri")]
        public async Task<IActionResult> GetBekleyenYatirimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _manuelYatirimTalepleriService.GetAllForWebSite(webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [PermissionAuthorize(Yetki.ManualYatirimlarOnaylananGorebilir)]
        [HttpGet("onaylanan-yatirim-talepleri")]
        public async Task<IActionResult> GetOnaylananYatirimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _manuelYatirimTalepleriService.GetAllForWebSite(webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Onaylandi, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [PermissionAuthorize(Yetki.ManualYatirimlarIptalGorebilir)]
        [HttpGet("red-yatirim-talepleri")]
        public async Task<IActionResult> GetRedYatirimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _manuelYatirimTalepleriService.GetAllForWebSite(webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Red, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(YatirimTalepleriManuelCreateDto yatirimTalepleriCreate)
        {
            if (!IsWebSiteUser())
            {
                throw new Exception("Bu işlemi sadece Web Site Kullanıcıları Yapabilir");
            }

            var webSiteId = GetWebSiteId();

            var result = await _manuelYatirimTalepleriService.Create(webSiteId, yatirimTalepleriCreate);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update-issendtowebsite-value")]
        public async Task<IActionResult> UpdateIsSendToWebsiteValue(Guid id)
        {
            var result = await _manuelYatirimTalepleriService.ChangeIsSendToWebSiteStatus(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }
    }
}
