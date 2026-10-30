namespace TikkoPayYonetici.Core.DTOs.WebSiteleri
{
    public class WebSiteleriListDto
    {
        public Guid Id { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public string? TelegramChannelId { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string KisaBaslik { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double OdemeKomisyonOrani { get; set; }
        public Guid? AffiliateId { get; set; }
        public double? AffiliateYaririmKomisyonOrani { get; set; }
        public double? AffiliateCekimKomisyonOrani { get; set; }
        public string AffiliateUnvan { get; set; } = string.Empty;
        public int Type { get; set; }
        public decimal BakiyeLimitor { get; set; }
        public decimal MinYatirimTutari { get; set; }
        public decimal MinCekimTutari { get; set; }
    }
}
