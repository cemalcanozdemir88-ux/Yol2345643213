using Microsoft.AspNetCore.Mvc;
using PayCoYonetici.Services.Interfaces;

namespace PayCoYonetici.Api.Controllers
{
    public class YatirimTalepleriController : BaseController
    {
        private readonly IYatirimTalepleriService _yatirimTalepleriService;

        public YatirimTalepleriController(IYatirimTalepleriService yatirimTalepleriService)
        {
            _yatirimTalepleriService = yatirimTalepleriService;
        }

        [HttpGet("onaylanan-yatirim-talepleri")]
        public async Task<IActionResult> GetOnaylananYatirimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);


            if (IsWebSiteUser())
            {
                var items = await _yatirimTalepleriService.GetAllForWebSite(webSiteId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _yatirimTalepleriService.GetAll(webSiteId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }


        }

        [HttpGet("get-toplam-yatirim-tutari")]
        public async Task<IActionResult> GetToplamYatirimTutari([FromQuery] Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var item = await _yatirimTalepleriService.GetToplamYatirimTutari(webSiteId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }


    }
}
