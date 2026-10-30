namespace TikkoPayYonetici.Core.DTOs.WebSiteleri
{
    public class WebSiteleriUpdateDto
    {
        public Guid Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string KisaBaslik { get; set; } = string.Empty;
        public string WebSiteUrl { get; set; } = string.Empty;
        public string TelegramChannelId { get; set; } = string.Empty;
        public Guid? AffiliateId { get; set; }
        public double? AffiliateYaririmKomisyonOrani { get; set; }
        public double? AffiliateCekimKomisyonOrani { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double OdemeKomisyonOrani { get; set; }
        public string ApiClientId { get; set; } = string.Empty;
        public string ApiClientSecret { get; set; } = string.Empty;
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
        public int Type { get; set; }
        public decimal BakiyeLimitor { get; set; }
        public decimal MinYatirimTutari { get; set; }
        public decimal MinCekimTutari { get; set; }
    }
}
