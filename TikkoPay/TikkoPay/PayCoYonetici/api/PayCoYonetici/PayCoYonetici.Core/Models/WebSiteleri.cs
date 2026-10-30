namespace PayCoYonetici.Core.Models
{
    public class WebSiteleri : BaseModel
    {
        public Guid? AffiliateId { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double OdemeKomisyonOrani { get; set; }
        public string ApiClientId { get; set; } = string.Empty;
        public string ApiClientSecret { get; set; } = string.Empty;
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
        public int Type { get; set; }

        public Affiliates? Affiliate { get; set; }
        public ICollection<WebSiteKasaHaraketleri> WebSiteKasaHaraketleriList { get; set; }
        public ICollection<Kullanicilar> Kullanicilar { get; set; }
        public ICollection<YatirimTalepleri> YatirimTalepleri { get; set; }
        public ICollection<CekimTalepleri> CekimTalepleri { get; set; }
    }
}
