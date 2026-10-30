using PayFixYonetici.Core.Enums;

namespace PayFixYonetici.Core.DTOs.Kullanicilar
{
    public class KullanicilarCreateDto
    {
        public Guid? WebSiteId { get; set; }
        public Guid? AffiliateId { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public KullaniciTip KullaniciTipi { get; set; }
        public bool IsOtpEnabled { get; set; }
        public bool Durum { get; set; }
    }
}
