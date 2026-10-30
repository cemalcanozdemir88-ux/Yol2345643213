using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri
{
    public class KullaniciIpAdresleriListDto
    {
        public Guid Id { get; set; }
        public Guid KullaniciId { get; set; }
        public Guid? TedarikciId { get; set; }
        public string? TedarikciAdi { get; set; }
        public Guid? WebsiteId { get; set; }
        public string? WebsiteAdi { get; set; }
        public string KullaniciEPosta { get; set; } = string.Empty;
        public string KullaniciAdSoyad { get; set; } = string.Empty;
        public KullaniciTip KullaniciTipi { get; set; }
        public string IpAdresi { get; set; } = string.Empty;
    }
}
