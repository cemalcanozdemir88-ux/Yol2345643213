using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.KasaHaraketleri;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
{
    public class KasaHaraketleriController : BaseController
    {
        private readonly IKasaHaraketleriService _kasaHaraketleriService;

        public KasaHaraketleriController(IKasaHaraketleriService kasaHaraketleriService)
        {
            _kasaHaraketleriService = kasaHaraketleriService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _kasaHaraketleriService.GetById(id);
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
        public async Task<IActionResult> GetAll(Guid? tedarikciId, int? durum, string createdDateStart, string createdDateEnd, int pageNumber, int recordsPerPage)
        {
            tedarikciId = GetTedarikciId();

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _kasaHaraketleriService.GetAll(tedarikciId, durum, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
            return Ok(items);
        }

        [HttpGet("get-toplam-kasa-haraketi-giris")]
        public async Task<IActionResult> GetToplamKasaHaraketiGiris()
        {
            var tedarikciId = GetTedarikciId();
            var sum = await _kasaHaraketleriService.GetToplamKasaHaraketiGiris(tedarikciId);
            return Ok(new ResponseViewModel<ToplamKasaHaraketiGirisDto>
            {
                Success = true,
                Result = new ToplamKasaHaraketiGirisDto
                {
                    Sum = sum
                }
            });
        }

        [HttpGet("get-toplam-kasa-haraketi-cikis")]
        public async Task<IActionResult> GetToplamKasaHaraketiCikis()
        {
            var tedarikciId = GetTedarikciId();
            var sum = await _kasaHaraketleriService.GetToplamKasaHaraketiCikis(tedarikciId);
            return Ok(new ResponseViewModel<ToplamKasaHaraketiCikisDto>
            {
                Success = true,
                Result = new ToplamKasaHaraketiCikisDto
                {
                    Sum = sum
                }
            });
        }

        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel(Guid? tedarikciId, int? durum, string createdDateStart, string createdDateEnd)
        {
            tedarikciId = GetTedarikciId();

            string base64String;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _kasaHaraketleriService.GetAll(tedarikciId, durum, _createdDateStart, _createdDateEnd);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "Tedarikçi";
                worksheet.Cell(1, 2).Value = "Açıklama";
                worksheet.Cell(1, 3).Value = "Tutar";
                worksheet.Cell(1, 4).Value = "Döviz Kuru";
                worksheet.Cell(1, 5).Value = "Usdt Miktar";
                worksheet.Cell(1, 6).Value = "Usdt Adres";
                worksheet.Cell(1, 7).Value = "Usdt Txt No";
                worksheet.Cell(1, 8).Value = "Resim Url";
                worksheet.Cell(1, 9).Value = "Durum";
                worksheet.Cell(1, 10).Value = "İşlem Tipi";
                worksheet.Cell(1, 11).Value = "Kayıt Tarihi";
                worksheet.Cell(1, 12).Value = "Kaydeden Kullanıcı";
                worksheet.Cell(1, 13).Value = "Güncelleme Tarihi";
                worksheet.Cell(1, 14).Value = "Güncelleyen Kullanıcı";

                int counter = 2;
                foreach (var item in items.Result)
                {
                    worksheet.Cell(counter, 1).Value = item.TedarikciAdi;
                    worksheet.Cell(counter, 2).Value = item.Aciklama;
                    worksheet.Cell(counter, 3).Value = item.Tutar;
                    worksheet.Cell(counter, 4).Value = item.DovizKuru;
                    worksheet.Cell(counter, 5).Value = item.UsdtMiktar;
                    worksheet.Cell(counter, 6).Value = item.UsdtAdres;
                    worksheet.Cell(counter, 7).Value = item.UsdtTxtNo;
                    worksheet.Cell(counter, 8).Value = item.ResimUrl;
                    worksheet.Cell(counter, 9).Value = item.Durum.ToString();
                    worksheet.Cell(counter, 10).Value = item.IslemTipi.ToString();
                    worksheet.Cell(counter, 11).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(counter, 12).Value = item.KaydedenKullanici;
                    worksheet.Cell(counter, 13).Value = item.GuncellemeTarihi?.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(counter, 14).Value = item.GuncelleyenKullanici;

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
                    fileName = $"KasaHaraketleri_{DateTime.Now:dd-MM-yyyy-HH-mm}.xlsx",
                    base64String = base64String
                }
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KasaHaraketleriCreateDto item)
        {
            var result = await _kasaHaraketleriService.Create(item);
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
        public async Task<IActionResult> Update(KasaHaraketleriUpdateDto item)
        {
            var result = await _kasaHaraketleriService.Update(item);
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
            var result = await _kasaHaraketleriService.Delete(id);
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
