using ParaYolYonetici.Core.Enums;

namespace ParaYolYonetici.Core.DTOs.Kullanicilar
{
    public class KullanicilarListDto
    {
        public Guid Id { get; set; }
        public Guid? TedarikciId { get; set; }
        public Guid? WebSiteId { get; set; }
        public Guid? AffiliateId { get; set; }
        public string? TedarikciAdi { get; set; } = string.Empty;
        public string? WebSiteUrl { get; set; } = string.Empty;
        public string? AffiliateUnvan { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public KullaniciTip KullaniciTipi { get; set; }
        public string KullaniciTipiAdi { get; set; } = string.Empty;
        public int IpAdresSayisi { get; set; }
        public bool IpLoggingEnabled { get; set; }
        public bool Durum { get; set; }
        public bool IsOtpEnabled { get; set; }
    }
}
