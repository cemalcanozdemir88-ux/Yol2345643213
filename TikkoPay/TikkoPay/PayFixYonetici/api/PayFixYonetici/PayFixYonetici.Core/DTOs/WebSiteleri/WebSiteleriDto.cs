namespace PayFixYonetici.Core.DTOs.WebSiteleri
{
    public class WebSiteleriDto
    {
        public Guid Id { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double OdemeKomisyonOrani { get; set; }
        public string ApiClientId { get; set; } = string.Empty;
        public string ApiClientSecret { get; set; } = string.Empty;
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
        public int Type { get; set; }

        public Guid AffiliateId { get; set; }
        public string AffiliateUnvan { get; set; } = string.Empty;
    }
}
