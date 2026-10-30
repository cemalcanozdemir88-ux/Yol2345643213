using PayCoYonetici.Core.Enums;

namespace PayCoYonetici.Core.DTOs.Kullanicilar
{
    public class KullanicilarListDto
    {
        public Guid Id { get; set; }
        public Guid? WebSiteId { get; set; }
        public Guid? AffiliateId { get; set; }
        public string? WebSiteUrl { get; set; } = string.Empty;
        public string? AffiliateUnvan { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public KullaniciTip KullaniciTipi { get; set; }
        public string KullaniciTipiAdi { get; set; } = string.Empty;
        public bool Durum { get; set; }
        public bool IsOtpEnabled { get; set; }
    }
}
