using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.YatirimTalepleri;
using PaparaTedarikciHizmet.Core.Enums;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Api.Controllers
{
    public class YatirimTalepleriController : BaseController
    {
        private readonly IYatirimTalepleriService _yatirimTalepleriService;

        public YatirimTalepleriController(IYatirimTalepleriService yatirimTalepleriService)
        {
            _yatirimTalepleriService = yatirimTalepleriService;
        }

        [HttpPut("approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var tedarikciId = GetTedarikciId();
            var result = await _yatirimTalepleriService.ApproveAsync(id, tedarikciId);
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
            var result = await _yatirimTalepleriService.CancelAsync(cancelRequest.Id, tedarikciId, cancelRequest.IptalAciklama);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] Guid? bankaId, string adSoyad, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, Durum durum)
        {
            string base64String;

            var tedarikciId = GetTedarikciId();

            adSoyad ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriService.GetAllAsync(tedarikciId, bankaId, adSoyad, durum, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "Talep No";
                worksheet.Cell(1, 2).Value = "Tedarikçi Adı";
                worksheet.Cell(1, 3).Value = "Banka Adı";
                worksheet.Cell(1, 4).Value = "Ad Soyad";
                worksheet.Cell(1, 5).Value = "Yöntem Tipi";
                worksheet.Cell(1, 6).Value = "Hesap Sahibi";
                worksheet.Cell(1, 7).Value = "Hesap No";
                worksheet.Cell(1, 8).Value = "Tutar";
                worksheet.Cell(1, 9).Value = "Tedarikci Komisyon";
                worksheet.Cell(1, 10).Value = "İşlem Tipi";
                worksheet.Cell(1, 11).Value = "Durum";
                worksheet.Cell(1, 12).Value = "Onay Tarihi";
                worksheet.Cell(1, 13).Value = "Kayıt Tarihi";

                int counter = 2;
                foreach (var item in items.Result)
                {
                    worksheet.Cell(counter, 1).Value = item.TalepNoWithFormated;
                    worksheet.Cell(counter, 2).Value = item.TedarikciAdi;
                    worksheet.Cell(counter, 3).Value = item.BankaAdi;
                    worksheet.Cell(counter, 4).Value = item.AdSoyad;
                    worksheet.Cell(counter, 5).Value = item.YontemTipi;
                    worksheet.Cell(counter, 6).Value = item.HesapSahibi;
                    worksheet.Cell(counter, 7).Value = item.HesapNo;
                    worksheet.Cell(counter, 8).Value = item.Tutar.ToString("0.00");
                    worksheet.Cell(counter, 9).Value = item.TedarikciKomisyon.ToString("0.00");
                    worksheet.Cell(counter, 10).Value = item.IslemTipi.ToString();
                    worksheet.Cell(counter, 11).Value = item.Durum.ToString();
                    worksheet.Cell(counter, 12).Value = item.OnayTarihi?.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(counter, 13).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");

                    counter++;
                }

                worksheet.Row(1).Style.Font.Bold = true;

                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                base64String = Convert.ToBase64String(ms.ToArray());
            }

            var result = new ResponseViewModel<ExcelExportDto>
            {
                Success = true,
                Result = new ExcelExportDto
                {
                    fileName = $"YatirimTalepleri_{DateTime.Now:dd-MM-yyyy-HH-mm}.xlsx",
                    base64String = base64String
                }
            };

            return Ok(result);
        }

        [HttpGet("get-pending-count")]
        public async Task<IActionResult> GetPendingCount()
        {
            var tedarikciId = GetTedarikciId();
            var count = await _yatirimTalepleriService.GetToplamBekleyenYatirimlar(tedarikciId);
            return Ok(count);
        }

        [HttpGet("bekleyen-yatirim-talepleri")]
        public async Task<IActionResult> GetBekleyenYatirimTalepleri([FromQuery] Guid? bankaId, string searchText, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
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

            var items = await _yatirimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
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

            var items = await _yatirimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("get-toplam-yatirim-tutari")]
        public async Task<IActionResult> GetToplamYatirimTutari([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var item = await _yatirimTalepleriService.GetToplamYatirimTutari(tedarikciId, bankaId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);
            return Ok(item);
        }

        [HttpGet("get-toplam-cekim-tutari")]
        public async Task<IActionResult> GetToplamCekimTutari([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var item = await _yatirimTalepleriService.GetToplamCekimTutari(tedarikciId, bankaId, searchText, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }
    }
}
