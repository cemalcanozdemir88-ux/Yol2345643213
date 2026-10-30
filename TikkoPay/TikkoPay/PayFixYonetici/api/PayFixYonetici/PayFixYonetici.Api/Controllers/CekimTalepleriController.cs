using Microsoft.AspNetCore.Mvc;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Api.Controllers
{
    public class CekimTalepleriController : BaseController
    {
        private readonly ICekimTalepleriService _cekimTalepleriService;

        public CekimTalepleriController(ICekimTalepleriService cekimTalepleriService)
        {
            _cekimTalepleriService = cekimTalepleriService;
        }

        [HttpGet("onaylanan-cekim-talepleri")]
        public async Task<IActionResult> GetOnaylananCekimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
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
                var items = await _cekimTalepleriService.GetAllForWebSite(webSiteId, searchText, true, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _cekimTalepleriService.GetAll(webSiteId, searchText, true, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
        }

        [HttpGet("red-cekim-talepleri")]
        public async Task<IActionResult> GetRedCekimTalepleri([FromQuery] Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
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
                var items = await _cekimTalepleriService.GetAllForWebSite(webSiteId, searchText, false, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _cekimTalepleriService.GetAll(webSiteId, searchText, false, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }


        }


        [HttpGet("get-toplam-cekim-tutari")]
        public async Task<IActionResult> GetToplamCekimTutari([FromQuery] Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd)
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

            var item = await _cekimTalepleriService.GetToplamCekimTutari(webSiteId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }

    }
}
