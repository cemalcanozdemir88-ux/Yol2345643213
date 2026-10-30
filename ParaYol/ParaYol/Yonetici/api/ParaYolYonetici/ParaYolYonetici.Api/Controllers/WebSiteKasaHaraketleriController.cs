using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using ParaYolYonetici.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using ParaYolYonetici.Core.DTOs;

namespace ParaYolYonetici.Api.Controllers
{
    public class WebSiteKasaHaraketleriController : BaseController
    {
        private IWebSiteKasaHaraketleriService _webSiteKasaHaraketleriService;

        public WebSiteKasaHaraketleriController(IWebSiteKasaHaraketleriService kasaHaraketleriService)
        {
            _webSiteKasaHaraketleriService = kasaHaraketleriService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _webSiteKasaHaraketleriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid? webSiteId, int? durum, int? islemTipi, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _webSiteKasaHaraketleriService.GetAll(webSiteId, durum, islemTipi, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel(Guid? webSiteId, int? durum, string? createdDateStart, string? createdDateEnd)
        {
            string base64String;

            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _webSiteKasaHaraketleriService.GetAll(webSiteId, durum, null, _createdDateStart, _createdDateEnd, 1, Int32.MaxValue);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "Web Sitesi";
                worksheet.Cell(1, 2).Value = "Açıklama";
                worksheet.Cell(1, 3).Value = "Tutar (TL)";
                worksheet.Cell(1, 4).Value = "Döviz Kuru";
                worksheet.Cell(1, 5).Value = "Usdt Miktar";
                worksheet.Cell(1, 6).Value = "Usdt Adres";
                worksheet.Cell(1, 7).Value = "Usdt Txt No";
                worksheet.Cell(1, 8).Value = "İşlem Tipi";
                worksheet.Cell(1, 9).Value = "Durum";
                worksheet.Cell(1, 10).Value = "Kaydeden Kullanıcı";
                worksheet.Cell(1, 11).Value = "Kayıt Tarihi";
                worksheet.Cell(1, 12).Value = "Güncelleyen Kullanıcı";
                worksheet.Cell(1, 13).Value = "Güncelleme Tarihi";

                int counter = 2;
                foreach (var item in items.Result)
                {
                    worksheet.Cell(counter, 1).Value = item.WebSiteKisaBaslik;
                    worksheet.Cell(counter, 2).Value = item.Aciklama;
                    worksheet.Cell(counter, 3).Value = item.Tutar;
                    worksheet.Cell(counter, 4).Value = item.DovizKuru;
                    worksheet.Cell(counter, 5).Value = item.UsdtMiktar;
                    worksheet.Cell(counter, 6).Value = item.UsdtAdres;
                    worksheet.Cell(counter, 7).Value = item.UsdtTxtNo;
                    worksheet.Cell(counter, 8).Value = item.IslemTipi.ToString();
                    worksheet.Cell(counter, 9).Value = item.Durum.ToString();
                    worksheet.Cell(counter, 10).Value = item.KaydedenKullanici;
                    worksheet.Cell(counter, 11).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(counter, 12).Value = item.GuncelleyenKullanici;
                    worksheet.Cell(counter, 13).Value = item.GuncellemeTarihi?.ToString("dd/MM/yyyy HH:mm");

                    counter++;
                }

                worksheet.Row(1).Style.Font.Bold = true;

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    base64String = Convert.ToBase64String(ms.ToArray());
                }
            }

            var result = new ResponseViewModel<ExcelExportDto>
            {
                Success = true,
                Result = new ExcelExportDto
                {
                    fileName = $"Website_KasaHaraketleri_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm")}.xlsx",
                    base64String = base64String
                }
            };

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(WebSiteKasaHaraketleriCreateDto item)
        {
            var result = await _webSiteKasaHaraketleriService.Create(item);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(WebSiteKasaHaraketleriUpdateDto item)
        {
            var result = await _webSiteKasaHaraketleriService.Update(item);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _webSiteKasaHaraketleriService.Delete(id);
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
