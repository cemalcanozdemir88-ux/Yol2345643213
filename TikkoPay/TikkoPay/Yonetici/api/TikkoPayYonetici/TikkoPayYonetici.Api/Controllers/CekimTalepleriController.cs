using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Api.ActionFilters.PermissionControl;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class CekimTalepleriController : BaseController
    {
        private readonly ICekimTalepleriService _cekimTalepleriService;
        private readonly ICekimTalepleriRepository _cekimTalepleriRepository;

        public CekimTalepleriController(ICekimTalepleriService cekimTalepleriService, ICekimTalepleriRepository cekimTalepleriRepository)
        {
            _cekimTalepleriService = cekimTalepleriService;
            _cekimTalepleriRepository = cekimTalepleriRepository;
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerCekimGuncelleyebilir)]
        [HttpPut("update-bekleyen-cekim")]
        public async Task<IActionResult> UpdateBekleyenCekim(UpdateBekleyenCekimDto request)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.UpdateBekleyenCekim(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerYonlendirebilir)]
        [HttpPut("update-dekont-kontrol")]
        public async Task<IActionResult> UpdateDekontKontrol(UpdateDekontKontrolDto request)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.UpdateDekontKontrol(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerYonlendirebilir)]
        [HttpPut("multi-tedarikciye-yonlendir")]
        public async Task<IActionResult> MultiTedarikciyeYonlendir(MultiTedarikciYonlendirmeDto request)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.MultiTedarikciyeYonlendirAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerCekimOnaylayabilir)]
        [HttpPut("onayla-siteye-bildir")]
        public async Task<IActionResult> OnaylaveSiteyeBildir(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.ApproveAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerCekimIptalEdebilir)]
        [HttpPut("iptalet-siteye-bildir")]
        public async Task<IActionResult> IptalEtveSiteyeBildir(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.CancelAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerTedarikciSilebilir)]
        [HttpPut("tedarikci-sil")]
        public async Task<IActionResult> TedarikciSil(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.TedarikciSil(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerManuelCekimlerEkleyebilir, Yetki.OnayliCekimlerManuelCekimlerEkleyebilir)]
        [HttpPost]
        public async Task<IActionResult> Create(CekimTalepleriCreateDto cekimTalepleriCreate)
        {
            if (!IsSuperAdmin())
            {
                cekimTalepleriCreate.WebSiteId = GetWebSiteId();
            }

            var result = await _cekimTalepleriService.CreateAsync(cekimTalepleriCreate);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerManuelCekimlerSilebilir, Yetki.OnayliCekimlerManuelCekimlerSilebilir)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!IsSuperAdmin())
            {
                throw new Exception("Yetki yok!");
            }

            var result = await _cekimTalepleriService.DeleteAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerExcelIndirebilir, Yetki.OnayliCekimlerExcelIndirebilir, Yetki.IptalEdilenCekimlerExcelIndirebilir)]
        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, Durum durum)
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
            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, durum, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (IsWebSiteUser())
                {
                    worksheet.Cell(1, 1).Value = "Talep No";
                    worksheet.Cell(1, 2).Value = "Transaction Id";
                    worksheet.Cell(1, 3).Value = "Banka Adı";
                    worksheet.Cell(1, 4).Value = "Web Site";

                    worksheet.Cell(1, 5).Value = "UserId";
                    worksheet.Cell(1, 6).Value = "UserName";

                    worksheet.Cell(1, 7).Value = "Ad Soyad";
                    worksheet.Cell(1, 8).Value = "Yöntem Tipi";
                    worksheet.Cell(1, 9).Value = "Hesap Sahibi";
                    worksheet.Cell(1, 10).Value = "Hesap No";
                    worksheet.Cell(1, 11).Value = "Tutar";
                    worksheet.Cell(1, 12).Value = "İşlem Tipi";
                    worksheet.Cell(1, 13).Value = "Durum";
                    worksheet.Cell(1, 14).Value = "Onay Tarihi";
                    worksheet.Cell(1, 15).Value = "Kayıt Tarihi";

                    int counter = 2;
                    foreach (var item in items.Result)
                    {
                        worksheet.Cell(counter, 1).Value = item.TalepNoWithFormated;
                        worksheet.Cell(counter, 2).Value = item.WebSiteTransactionId;
                        worksheet.Cell(counter, 3).Value = item.BankaAdi;
                        worksheet.Cell(counter, 4).Value = item.WebSiteUrl;

                        worksheet.Cell(counter, 5).Value = item.UserId;
                        worksheet.Cell(counter, 6).Value = item.UserName;

                        worksheet.Cell(counter, 7).Value = item.AdSoyad;
                        worksheet.Cell(counter, 8).Value = item.YontemTipi;
                        worksheet.Cell(counter, 9).Value = item.HesapSahibi;
                        worksheet.Cell(counter, 10).Value = item.HesapNo;
                        worksheet.Cell(counter, 11).Value = item.Tutar;
                        worksheet.Cell(counter, 12).Value = item.IslemTipi.ToString();
                        worksheet.Cell(counter, 13).Value = item.Durum.ToString();
                        worksheet.Cell(counter, 14).Value = item.OnayTarihi?.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(counter, 15).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");

                        counter++;
                    }
                }
                else
                {


                    worksheet.Cell(1, 1).Value = "Talep No";
                    worksheet.Cell(1, 2).Value = "Payment Method Id";
                    worksheet.Cell(1, 3).Value = "Transaction Id";
                    worksheet.Cell(1, 4).Value = "Tedarikçi Adı";
                    worksheet.Cell(1, 5).Value = "Banka Adı";
                    worksheet.Cell(1, 6).Value = "Web Site";
                    worksheet.Cell(1, 7).Value = "Ad Soyad";
                    worksheet.Cell(1, 8).Value = "UserId";
                    worksheet.Cell(1, 9).Value = "UserName";
                    worksheet.Cell(1, 10).Value = "Yöntem Tipi";
                    worksheet.Cell(1, 11).Value = "Hesap Sahibi";
                    worksheet.Cell(1, 12).Value = "Hesap No";
                    worksheet.Cell(1, 13).Value = "Tutar";
                    worksheet.Cell(1, 14).Value = "Tedarikci Komisyon";
                    worksheet.Cell(1, 15).Value = "Toplam";
                    worksheet.Cell(1, 16).Value = "İşlem Tipi";
                    worksheet.Cell(1, 17).Value = "Durum";
                    worksheet.Cell(1, 18).Value = "Onay Tarihi";
                    worksheet.Cell(1, 19).Value = "Kayıt Tarihi";

                    int counter = 2;
                    foreach (var item in items.Result)
                    {
                        worksheet.Cell(counter, 1).Value = item.TalepNoWithFormated;
                        worksheet.Cell(counter, 2).Value = item.PaymentMethodId;
                        worksheet.Cell(counter, 3).Value = item.WebSiteTransactionId;
                        worksheet.Cell(counter, 4).Value = item.TedarikciAdi;
                        worksheet.Cell(counter, 5).Value = item.BankaAdi;
                        worksheet.Cell(counter, 6).Value = item.WebSiteUrl;
                        worksheet.Cell(counter, 7).Value = item.AdSoyad;
                        worksheet.Cell(counter, 8).Value = item.UserId;
                        worksheet.Cell(counter, 9).Value = item.UserName;
                        worksheet.Cell(counter, 10).Value = item.YontemTipi;
                        worksheet.Cell(counter, 11).Value = item.HesapSahibi;
                        worksheet.Cell(counter, 12).Value = item.HesapNo;
                        worksheet.Cell(counter, 13).Value = item.Tutar;
                        worksheet.Cell(counter, 14).Value = item.TedarikciKomisyon;
                        worksheet.Cell(counter, 15).Value = item.Toplam;
                        worksheet.Cell(counter, 16).Value = item.IslemTipi.ToString();
                        worksheet.Cell(counter, 17).Value = item.Durum.ToString();
                        worksheet.Cell(counter, 18).Value = item.OnayTarihi?.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(counter, 19).Value = item.KayitTarihi.ToString("dd/MM/yyyy HH:mm");

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
                    fileName = $"CekimTalepleri_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm")}.xlsx",
                    base64String = base64String
                }
            };

            return Ok(result);
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerGorebilir)]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();

            var items = await _cekimTalepleriService.GetByIdAsync(id);
            return Ok(items);
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerYonlendirebilir, Yetki.BekleyenCekimlerGorebilir)]
        [HttpGet("get-by-topid/{topId}")]
        public async Task<IActionResult> GetByTopId(Guid topId)
        {
            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();

            var items = await _cekimTalepleriService.GetByTopIdAsync(topId, showWebSite);
            return Ok(items);
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerGorebilir)]
        [HttpGet("get-dekont/{id}")]
        public async Task<IActionResult> GetDekont(Guid id)
        {
            var item = await _cekimTalepleriService.GetDekontByIdAsync(id);
            return Ok(item);
        }

        [PermissionAuthorize(Yetki.BekleyenCekimlerGorebilir)]
        [HttpGet("bekleyen-cekim-talepleri")]
        public async Task<IActionResult> GetBekleyenCekimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage, int sortColumn)
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
                var items = await _cekimTalepleriService.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, null, Core.Enums.Durum.Bekliyor, null, null, _createdDateStart, _createdDateEnd, sortColumn, pageNumber, recordsPerPage);
                return Ok(items);
            }
        }

        [PermissionAuthorize(Yetki.OnayliCekimlerGorebilir)]
        [HttpGet("onaylanan-cekim-talepleri")]
        public async Task<IActionResult> GetOnaylananCekimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? dekontKontrolDurum, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
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

            DekontKontrolDurum? dekontKontrolDurumEnum = null;
            if (!string.IsNullOrWhiteSpace(dekontKontrolDurum))
            {
                if (dekontKontrolDurum.Equals("0"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.Bekliyor;
                }
                else if (dekontKontrolDurum.Equals("1"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.DekontDogru;
                }
                else if (dekontKontrolDurum.Equals("2"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.DekontHatali;
                }
            }

            if (IsWebSiteUser())
            {
                var items = await _cekimTalepleriService.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, dekontKontrolDurumEnum, Core.Enums.Durum.Onaylandi, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, (int)OrderBy.OnayTarihi, pageNumber, recordsPerPage);
                return Ok(items);
            }
        }

        [PermissionAuthorize(Yetki.TumCekimlerGorebilir)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, Durum? durum, string? dekontKontrolDurum, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            if (!IsSuperAdmin())
            {
                return BadRequest();
            }

            bool showWebSite = IsSuperAdmin() || IsWebSiteUser();
            searchText ??= string.Empty;

            DateTime? _createdDateStart = StringToDateTime(createdDateStart);
            DateTime? _createdDateEnd = StringToDateTime(createdDateEnd);

            DekontKontrolDurum? dekontKontrolDurumEnum = null;
            if (!string.IsNullOrWhiteSpace(dekontKontrolDurum))
            {
                if (dekontKontrolDurum.Equals("0"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.Bekliyor;
                }
                else if (dekontKontrolDurum.Equals("1"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.DekontDogru;
                }
                else if (dekontKontrolDurum.Equals("2"))
                {
                    dekontKontrolDurumEnum = DekontKontrolDurum.DekontHatali;
                }
            }

            var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, dekontKontrolDurumEnum, durum, null, null, _createdDateStart, _createdDateEnd, (int)OrderBy.KayitTarihi, pageNumber, recordsPerPage);
            return Ok(items);
        }


        [PermissionAuthorize(Yetki.IptalEdilenCekimlerGorebilir)]
        [HttpGet("red-cekim-talepleri")]
        public async Task<IActionResult> GetRedCekimTalepleri([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd, int pageNumber, int recordsPerPage)
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
                var items = await _cekimTalepleriService.GetAllForWebSiteAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, pageNumber, recordsPerPage);
                return Ok(items);
            }
            else
            {
                var items = await _cekimTalepleriService.GetAllAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), showWebSite, null, Core.Enums.Durum.Red, _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd, (int)OrderBy.OnayTarihi, pageNumber, recordsPerPage);
                return Ok(items);
            }


        }

        [PermissionAuthorize(Yetki.OnayliCekimlerGorebilir, Yetki.AnaSayfaHesapOzetiGorebilir)]
        [HttpGet("get-toplam-cekim-tutari")]
        public async Task<IActionResult> GetToplamCekimTutari([FromQuery] Guid? tedarikciId, Guid? bankaId, Guid? webSiteId, string? searchText, string? approveDateStart, string? approveDateEnd, string? createdDateStart, string? createdDateEnd)
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

            var item = await _cekimTalepleriService.GetToplamCekimTutariAsync(tedarikciId, bankaId, webSiteId, searchText.Trim(), _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }

        [PermissionAuthorize(Yetki.OnayliCekimlerGorebilir, Yetki.AnaSayfaHesapOzetiGorebilir)]
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

            var item = await _cekimTalepleriService.GetOrtalamaOnaySuresi(tedarikciId, bankaId, webSiteId, searchText.Trim(), _approveDateStart, _approveDateEnd, _createdDateStart, _createdDateEnd);

            return Ok(item);
        }

    }
}
