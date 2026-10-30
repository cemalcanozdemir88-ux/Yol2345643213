using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
{
    public class YatirimTalepleriManualController : BaseController
    {
        private readonly IYatirimTalepleriManualService _yatirimTalepleriManualService;

        public YatirimTalepleriManualController(IYatirimTalepleriManualService yatirimTalepleriManualService)
        {
            _yatirimTalepleriManualService = yatirimTalepleriManualService;
        }

        [HttpPut("approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var tedarikciId = GetTedarikciId();
            var result = await _yatirimTalepleriManualService.ApproveAsync(id, tedarikciId);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("cancel")]
        public async Task<IActionResult> Cancel(CancelRequest cancelRequest)
        {
            var tedarikciId = GetTedarikciId();
            var result = await _yatirimTalepleriManualService.CancelAsync(cancelRequest.Id, tedarikciId, cancelRequest.IptalAciklama);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("bekleyen-yatirim-talepleri")]
        public async Task<IActionResult> GetBekleyenYatirimTalepleri([FromQuery] Guid? bankaId, string searchText, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriManualService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("onaylanan-yatirim-talepleri")]
        public async Task<IActionResult> GetOnaylananYatirimTalepleri([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriManualService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("red-yatirim-talepleri")]
        public async Task<IActionResult> GetRedYatirimTalepleri([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriManualService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

    }
}
