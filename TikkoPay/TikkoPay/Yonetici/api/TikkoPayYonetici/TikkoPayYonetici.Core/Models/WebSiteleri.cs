namespace TikkoPayYonetici.Core.Models
{
    public class WebSiteleri : BaseModel
    {
        public Guid? AffiliateId { get; set; }
        public string? Baslik { get; set; } = string.Empty;
        public string? KisaBaslik { get; set; } = string.Empty;
        public string WebSiteUrl { get; set; } = string.Empty;
        public string? TelegramChannelId { get; set; } = string.Empty;
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

        public Affiliates? Affiliate { get; set; }
        public ICollection<WebSiteKasaHaraketleri> WebSiteKasaHaraketleriList { get; set; }
        public ICollection<Kullanicilar> Kullanicilar { get; set; }
        public ICollection<YatirimCekimTalepleri> YatirimTalepleri { get; set; }
        public ICollection<YatirimCekimTalepleriManual> YatirimTalepleriManual { get; set; }
        public ICollection<KullaniciBlackLists> KullaniciBlackLists { get; set; }
        public ICollection<WebSiteTedarikcileri> WebSiteTedarikcileriList { get; set; }
        public ICollection<WebSiteTedarikcileriManual> WebSiteTedarikcileriManualList { get; set; }
        public ICollection<WebSiteMutabakatlari> WebSiteMutabakatlariList { get; set; }
    }
}
