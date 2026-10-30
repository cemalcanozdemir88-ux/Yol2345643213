using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Api.ActionFilters.PermissionControl;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class YatirimTalepleriController : BaseController
    {
        private readonly IYatirimTalepleriService _yatirimTalepleriService;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;

        public YatirimTalepleriController(IYatirimTalepleriService yatirimTalepleriService,
                                          IYatirimTalepleriRepository yatirimTalepleriRepository)
        {
            _yatirimTalepleriService = yatirimTalepleriService;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
        }

        [PermissionAuthorize(Yetki.BekleyenYatirimlarDuzenleyebilir)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _yatirimTalepleriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [PermissionAuthorize(Yetki.ManuelYatirimlarEkleyebilir)]
        [HttpPost]
        public async Task<IActionResult> Create(YatirimTalepleriCreateDto yatirimTalepleriCreate)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _yatirimTalepleriService.Create(yatirimTalepleriCreate);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.IptalEdilenYatirimlariBekleyeneAlabilir)]
        [HttpPut("bekleyene-al")]
        public async Task<IActionResult> BekleyeneAl(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _yatirimTalepleriService.BekleyeneAl(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenYatirimlarDuzenleyebilir)]
        [HttpPut("update-price")]
        public async Task<IActionResult> UpdatePrice(Guid id, decimal price)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _yatirimTalepleriService.UpdatePrice(id, price);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.ManuelYatirimlarSilebilir)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _yatirimTalepleriService.Delete(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.OnayliYatirimlarExcelIndirebilir, Yetki.IptalEdilenYatirimlarExcelIndirebilir)]
        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, IslemTip islemTipi, Durum durum)
        {
            string base64String;

            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            var items = await _yatirimTalepleriService.GetAll(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, durum, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (IsWebSiteUser())
                {
                    worksheet.Cell(1, 1).Value = "Talep No";
                    worksheet.Cell(1, 2).Value = "Transaction Id";
                    worksheet.Cell(1, 3).Value = "Banka Adı";
                    worksheet.Cell(1, 4).Value = "Web Site";
                    worksheet.Cell(1, 5).Value = "Ad Soyad";
                    worksheet.Cell(1, 6).Value = "Yöntem Tipi";
                    worksheet.Cell(1, 7).Value = "Hesap Sahibi";
                    worksheet.Cell(1, 8).Value = "Hesap No";
                    worksheet.Cell(1, 9).Value = "Tutar";
                    worksheet.Cell(1, 10).Value = "İşlem Tipi";
                    worksheet.Cell(1, 11).Value = "Durum";
                    worksheet.Cell(1, 12).Value = "Onay Tarihi";
                    worksheet.Cell(1, 13).Value = "Kayıt Tarihi";

                    int counter = 2;
                    foreach (var item in items.Result)
                    {
                        worksheet.Cell(counter, 1).Value = item.TalepNoWithFormated;
                        worksheet.Cell(counter, 2).Value = item.WebSiteTransactionId;
                        worksheet.Cell(counter, 3).Value = item.BankaAdi;
                        worksheet.Cell(counter, 4).Value = item.WebSiteUrl;
                        worksheet.Cell(counter, 5).Value = item.AdSoyad;
                        worksheet.Cell(counter, 6).Value = item.YontemTipi;
                        worksheet.Cell(counter, 7).Value = item.HesapSahibi;
                        worksheet.Cell(counter, 8).Value = item.HesapNo;
                        worksheet.Cell(counter, 9).Value = item.Tutar;
                        worksheet.Cell(counter, 10).Value = item.IslemTipi.ToString();
                        worksheet.Cell(counter, 11).Value = item.Durum.ToString();
                        worksheet.Cell(counter, 12).Value = item.OnayTarihi?.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(counter, 13).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");

                        counter++;
                    }
                }
                else
                {
                    

                    worksheet.Cell(1, 1).Value = "Talep No";
                    worksheet.Cell(1, 2).Value = "Transaction Id";
                    worksheet.Cell(1, 3).Value = "Tedarikçi Adı";
                    worksheet.Cell(1, 4).Value = "Banka Adı";
                    worksheet.Cell(1, 5).Value = "Web Site";
                    worksheet.Cell(1, 6).Value = "Ad Soyad";
                    worksheet.Cell(1, 7).Value = "UserId";
                    worksheet.Cell(1, 8).Value = "UserName";
                    worksheet.Cell(1, 9).Value = "Yöntem Tipi";
                    worksheet.Cell(1, 10).Value = "Hesap Sahibi";
                    worksheet.Cell(1, 11).Value = "Hesap No";
                    worksheet.Cell(1, 12).Value = "Tutar";
                    worksheet.Cell(1, 13).Value = "Tedarikci Komisyon";
                    worksheet.Cell(1, 14).Value = "Toplam";
                    worksheet.Cell(1, 15).Value = "İşlem Tipi";
                    worksheet.Cell(1, 16).Value = "Durum";
                    worksheet.Cell(1, 17).Value = "Onay Tarihi";
                    worksheet.Cell(1, 18).Value = "Kayıt Tarihi";

                    int counter = 2;
                    foreach (var item in items.Result)
                    {
                        worksheet.Cell(counter, 1).Value = item.TalepNoWithFormated;
                        worksheet.Cell(counter, 2).Value = item.WebSiteTransactionId;
                        worksheet.Cell(counter, 3).Value = item.TedarikciAdi;
                        worksheet.Cell(counter, 4).Value = item.BankaAdi;
                        worksheet.Cell(counter, 5).Value = item.WebSiteUrl;
                        worksheet.Cell(counter, 6).Value = item.AdSoyad;
                        worksheet.Cell(counter, 7).Value = item.UserId;
                        worksheet.Cell(counter, 8).Value = item.UserName;
                        worksheet.Cell(counter, 9).Value = item.YontemTipi;
                        worksheet.Cell(counter, 10).Value = item.HesapSahibi;
                        worksheet.Cell(counter, 11).Value = item.HesapNo;
                        worksheet.Cell(counter, 12).Value = item.Tutar;
                        worksheet.Cell(counter, 13).Value = item.TedarikciKomisyon;
                        worksheet.Cell(counter, 14).Value = item.Toplam;
                        worksheet.Cell(counter, 15).Value = item.IslemTipi.ToString();
                        worksheet.Cell(counter, 16).Value = item.Durum.ToString();
                        worksheet.Cell(counter, 17).Value = item.OnayTarihi?.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(counter, 18).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");

                        counter++;
                    }

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
                    fileName = $"YatirimTalepleri_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm")}.xlsx",
                    base64String = base64String
                }
            };

            return Ok(result);
        }

        [PermissionAuthorize(Yetki.BekleyenYatirimlarGorebilir)]
        [HttpGet("bekleyen-yatirim-talepleri")]
        public async Task<IActionResult> GetBekleyenYatirimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            if (IsWebSiteUser())
            {
                var items = await _yatirimTalepleriService.GetAllForWebSite(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _yatirimTalepleriService.GetAll(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }  
        }

        [PermissionAuthorize(Yetki.OnayliYatirimlarGorebilir)]
        [HttpGet("onaylanan-yatirim-talepleri")]
        public async Task<IActionResult> GetOnaylananYatirimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            
            if(IsWebSiteUser())
            {
                var items = await _yatirimTalepleriService.GetAllForWebSite(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _yatirimTalepleriService.GetAll(tedarikciId, bankaId, webSiteId, searchText.Trim(), Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, (int)OrderBy.OnayTarihi, pageNumber, recordsPerPage);
                return Ok(items);
            }
        }

        [PermissionAuthorize(Yetki.TumYatirimlarGorebilir)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, Durum? durum, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                return BadRequest();
            }

            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            var items = await _yatirimTalepleriService.GetAll(tedarikciId, bankaId, webSiteId, searchText.Trim(), durum, null, null, _createdDateStart, _createdDateEnd, orderBy: (int)OrderBy.KayitTarihi, pageNumber: pageNumber, recordsPerPage: recordsPerPage);
            return Ok(items);
        }

        [PermissionAuthorize(Yetki.IptalEdilenYatirimlarGorebilir)]
        [HttpGet("red-yatirim-talepleri")]
        public async Task<IActionResult> GetRedYatirimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                webSiteId = GetWebSiteId();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _approveDateStart = StringToDateTime(approveDateStart);
            DateTime? _approveDateEnd = StringToDateTime(approveDateEnd);

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            if (IsWebSiteUser())
            {
                var items = await _yatirimTalepleriService.GetAllForWebSite(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _yatirimTalepleriService.GetAll(tedarikciId, bankaId, webSiteId, searchText.Trim(), Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, (int)OrderBy.OnayTarihi, pageNumber, recordsPerPage);
                return Ok(items);
            }   
        }

        [PermissionAuthorize(Yetki.OnayliYatirimlarGorebilir, Yetki.AnaSayfaHesapOzetiGorebilir)]
        [HttpGet("get-toplam-yatirim-tutari")]
        public async Task<IActionResult> GetToplamYatirimTutari([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd)
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

            var item = await _yatirimTalepleriService.GetToplamYatirimTutari(tedarikciId, bankaId, webSiteId, searchText.Trim(), _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }

        [PermissionAuthorize(Yetki.OnayliYatirimlarGorebilir, Yetki.AnaSayfaHesapOzetiGorebilir)]
        [HttpGet("get-ortalama-onay-suresi")]
        public async Task<IActionResult> GetOrtalamaOnaySuresi([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd)
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

            var item = await _yatirimTalepleriService.GetOrtalamaOnaySuresi(tedarikciId, bankaId, webSiteId, searchText.Trim(), _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }
    }
}
