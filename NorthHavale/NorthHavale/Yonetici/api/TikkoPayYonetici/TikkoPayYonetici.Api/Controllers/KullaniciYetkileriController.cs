using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.KullaniciYetkileri;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    public class KullaniciYetkileriController : BaseController
    {
        private readonly IKullaniciYetkileriService _kullaniciYetkileriService;
        private readonly IKullanicilarService _kullanicilarService;

        public KullaniciYetkileriController(IKullaniciYetkileriService kullaniciYetkileriService, 
                                            IKullanicilarService kullanicilarService)
        {
            _kullaniciYetkileriService = kullaniciYetkileriService;
            _kullanicilarService = kullanicilarService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _kullaniciYetkileriService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet("get-by-kullaniciId")]
        public async Task<IActionResult> GetByKullaniciId([FromQuery] Guid kullaniciId)
        {
            var items = new ResponseViewModel<List<KullaniciYetkileriListDto>>();

            var kullaniciItem = await _kullanicilarService.GetById(kullaniciId);
            if (kullaniciItem.Success)
            {
                items = await _kullaniciYetkileriService.GetByKullaniciId(kullaniciId);
                if (kullaniciItem.Result.KullaniciTipi == Core.Enums.KullaniciTip.WebSiteKullanicisi)
                {
                    RemoveKasaYonetimi(items);
                    RemoveYatirimlar(items);
                    RemoveCekimler(items);
                    RemoveBankaYonetimi(items);
                    RemoveTedarikciYonetimi(items);
                    RemoveSiteTanimlari(items);
                    RemoveAffiliateYonetimi(items);
                }

                
            }

            return Ok(items);
        }

        private static void RemoveKasaYonetimi(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("5538C01C-E2D4-4013-B555-77389B1334B8"), // Kasa Yonetimi
                Guid.Parse("FEF925E7-3E36-4C54-A767-0B6F1C388084"), // Tedarikçi Kasa Haraketleri
                Guid.Parse("9B2E8969-F00D-4362-9F31-17C422122B3D"), // Tedarikçi Kasa Haraketleri Görebilir
                Guid.Parse("CD056F8F-9AC2-418A-9EC0-1BFCC94EE6AE"), // Tedarikçi Kasa Haraketleri Excel İndirebilir
                Guid.Parse("43F98A4A-1A74-4E70-B954-59E86921DF51"), // Tedarikçi Kasa Haraketleri Düzenleyebilir
                Guid.Parse("9317B80F-71B0-47B9-896F-D980AC7A4C34"), // Tedarikçi Kasa Haraketleri Ekleyebilir

                Guid.Parse("F83E3FEC-A4C7-44BE-A889-E5BDC1447B9B"), // Tedarikçi Bakiye
                Guid.Parse("504594F9-C11F-4CDD-94F0-47ADEAF2EA37"), // Tedarikçi Bakiye Borçlu Yapabilir
                Guid.Parse("C2924FFC-E6E8-4AE3-AF6B-4BE5F2E640E8"), // Tedarikçi Bakiye Alacaklı Yapabilir
                Guid.Parse("7A21FE34-827D-484B-B70D-AE179AA835FD"), // Tedarikçi Bakiye Görebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveYatirimlar(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("C0CBF579-F26F-4F53-81C6-0C3BDB3CF1BC"), // Bekleyen Yatırımlar Düzenleyebilir
                Guid.Parse("424F0E3E-B858-40EB-96E3-93B4903BAC8F"), // Bekleyen Yatırımlar Excel İndirebilir

                //Guid.Parse("1E66682C-BACD-4D9C-9745-A6C7285A1740"), // Onaylı Yatırımlar Excel İndirebilir
                Guid.Parse("19F9B205-00C8-4EDC-98B8-A741D01AD387"), // Manuel Yatırım Ekleyebilir
                Guid.Parse("4B6E7D18-B9B1-48CD-AD2F-DAC9517D5C0B"), // Manuel Yatırım Silebilir

                //Guid.Parse("1F7F63CC-2371-46FF-9A63-9D8508013C81"), // İptal Edilen Yatırımlar Excel İndirebilir
                Guid.Parse("B5981CAB-1C99-4765-9401-B9E0FAC9A8A1"), // İptal Edilen Yatırımları Bekleyene Alabilir

                Guid.Parse("9E3B1FF3-23D0-4374-886F-4A7474A6EDC3"), // Tum Yatırımlar Görebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveCekimler(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("608DB271-4F5E-47C9-AE4B-50406A8DF998"), // Bekleyen Çekimler Yönlendirebilir
                Guid.Parse("023A7257-7F87-4C4C-B099-DD22696D8D4F"), // Bekleyen Çekimler Excel İndirebilir

                //Guid.Parse("926F2A2E-239A-4DFF-8826-0E4251489555"), // Onaylı Çekimler Excel İndirebilir
                Guid.Parse("29A89D93-EABC-49A5-B953-2BF50671C1F0"), // Maneul Çekim Ekleybilir
                Guid.Parse("5E89F0EB-D0E1-4E43-98D1-A6263E4F1F28"), // Manuel Çekim Silebilir

                //Guid.Parse("FC23B88A-5CD1-4BB0-BF80-1D936716D40D"), // İptal Edilen Çekimler Excel İndirebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveBankaYonetimi(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("387322B4-4A45-4474-BFF8-71F227CB38AE"), // Banka Yönetimi

                Guid.Parse("AFB2523C-948D-4A6A-B272-78D0DED4EB35"), // Banka Tanımları Ekleyebilir
                Guid.Parse("E015FBFC-83FF-430C-BCD5-792DCB387956"), // Banka Tanımları Görebilir
                Guid.Parse("42E19B72-6228-47A4-877B-96730E8374A0"), // Banka Tanımları Silebilir
                Guid.Parse("A9605BEA-E700-474B-9E28-CB01B0F0B5A9"), // Banka Tanımları
                Guid.Parse("41ED22B3-18AD-4488-8718-D7A3D7C65116"), // Banka Tanımları Düzenleyebilir

                Guid.Parse("5F8200B1-8994-464E-B936-22C36D9B2A44"),  // Havale Bankaları
                Guid.Parse("E31B6394-B200-4929-A4E8-8888AE88F2FB"),  // Havale Bankaları Düzenleyebilir
                Guid.Parse("82779526-FA1C-4C42-966A-A8082C7000B1"),  // Havale Bankaları Görebilir
                Guid.Parse("48873056-BA37-46E5-ABCF-E88924F7337D"),  // Havale Bankaları Silebilir
                Guid.Parse("00089569-FD6D-4AC7-A885-FFDF9468A4B6"),  // Havale Bankaları Ekleyebilir


            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveTedarikciYonetimi(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("1F208BB6-1D6A-47AF-8FF8-053DDCE5307F"), // Tedarikçi Yönetimi

                Guid.Parse("A8836E1D-6E76-4B0A-A597-23046985EB36"), // Tedarikçi Tanımları
                Guid.Parse("6773A005-2711-4E04-A9E3-811310A2D0E0"), // Tedarikçi Tanımları Görebilir
                Guid.Parse("920FBA84-F07A-48A2-AA4E-6B3DF3E66846"), // Tedarikçi Ekleyebilir
                Guid.Parse("5975177C-4EA5-4B7E-8AEE-1F557C650F0A"), // Tedarikçi Düzenleyebilir
                Guid.Parse("B8110020-7B41-41FF-AB6D-875A40611967"), // Tedarikçi Silebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveSiteTanimlari(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("1A86378B-CEF6-4262-95AE-187619909F86"), // Site Tanımları

                Guid.Parse("FE9C43C3-41D3-4D7F-AAC2-437FB0A6A40D"), // Site Tanımları Görebilir
                Guid.Parse("A25A803D-3AEE-4B92-8BEF-19481A87EF0B"), // Web Sitesi Düzenleyebilir
                Guid.Parse("D0E2AD3C-340C-49BD-A108-DC2ABF1BE5CA"), // Web Sitesi Silebilir
                Guid.Parse("6DB0A7B5-42CA-4F66-8780-F6CA798565C1"), // Web Sitesi Ekleyebilir

            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        private static void RemoveAffiliateYonetimi(ResponseViewModel<List<KullaniciYetkileriListDto>> items)
        {
            var yetkiIds = new List<Guid>
            {
                Guid.Parse("11D4E951-44DA-45AF-B5EB-511C7590917E"), // Affiliate Yönetimi

                Guid.Parse("CDDEE3F8-F056-40AE-BD12-C69D1D23AAA6"), // Affiliate Tanımları
                Guid.Parse("FEBBAFB1-3969-4CC4-95B2-F801459478F2"), // Affiliate Tanımları Görebilir
                Guid.Parse("164DF90B-E47D-47D6-B4A9-5FEE39988B26"), // Affiliate Ekleyebilir
                Guid.Parse("484C2B95-9357-400E-B961-D2A6612AF955"), // Affiliate Düzenleyebilir
                Guid.Parse("34A14D83-B1E1-46E0-B93B-751AF5834C33"), // Affiliate Silebilir
                
                Guid.Parse("AB746774-5894-4DFE-803E-74A5645BDEA4"), // Affiliate Bakiye
                Guid.Parse("568A803A-94B6-4A80-8BCE-782E7DE096EB"), // Affiliate Bakiye Görebilir
            };

            foreach (var yetkiId in yetkiIds)
            {
                var itemToRemove = items.Result.FirstOrDefault(k => k.YetkiId == yetkiId);
                if (itemToRemove != null)
                {
                    items.Result.Remove(itemToRemove);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(KullaniciYetkileriCreateDto createDto)
        {
            var result = await _kullaniciYetkileriService.Create(createDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(KullaniciYetkileriDeleteDto deleteDto)
        {
            var result = await _kullaniciYetkileriService.Delete(deleteDto);
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
