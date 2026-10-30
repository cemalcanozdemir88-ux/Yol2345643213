using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
{
    public class CekimTalepleriController : BaseController
    {
        private readonly ICekimTalepleriService _cekimTalepleriService;
        private readonly IImageUploadService _imageUploadService;

        public CekimTalepleriController(ICekimTalepleriService cekimTalepleriService,
                                        IImageUploadService imageUploadService)
        {
            _cekimTalepleriService = cekimTalepleriService;
            _imageUploadService = imageUploadService;
        }

        [HttpPost("approve")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Approve([FromForm] Guid id, [FromForm] IList<IFormFile> dekontDosyasi)
        {
            if (dekontDosyasi == null || dekontDosyasi.Count == 0)
            {
                return BadRequest(new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "En az bir dekont dosyası seçiniz!"
                });
            }

            var tedarikciId = GetTedarikciId();
            var dekontDosyaYollari = new List<string>();

            try
            {
                var fileBytesList = new List<(byte[] File, string ContentType)>();

                foreach (var file in dekontDosyasi)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    fileBytesList.Add((memoryStream.ToArray(), file.ContentType));
                }

                var urls = await _imageUploadService.UploadImagesAsync(fileBytesList);
                dekontDosyaYollari.AddRange(urls);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = $"Dekont yüklenirken hata oluştu: {ex.Message}"
                });
            }

            var dekontDosyaYolu = string.Join(" ", dekontDosyaYollari);

            var result = await _cekimTalepleriService.ApproveAsync(id, tedarikciId, dekontDosyaYolu);
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
            var result = await _cekimTalepleriService.CancelAsync(cancelRequest.Id, tedarikciId, cancelRequest.IptalAciklama);
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
        public async Task<IActionResult> ExportToExcel([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, Durum durum)
        {
            string base64String;

            var tedarikciId = GetTedarikciId();

            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, durum, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

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

        [HttpGet("bekleyen-cekim-talepleri")]
        public async Task<IActionResult> GetBekleyenCekimTalepleri([FromQuery] Guid? bankaId, string searchText, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("onaylanan-cekim-talepleri")]
        public async Task<IActionResult> GetOnaylananCekimTalepleri([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("red-cekim-talepleri")]
        public async Task<IActionResult> GetRedCekimTalepleri([FromQuery] Guid? bankaId, string searchText, string approveDateStart, string approveDateEnd, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var tedarikciId = GetTedarikciId();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, searchText, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }
    }
}
