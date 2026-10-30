namespace PayCoYonetici.Core.Models
{
    public class CekimTalepleri : BaseModel
    {
        public int IdNumber { get; set; }
        public string TalepNo { get; set; } = string.Empty;
        public Guid? WebSiteId { get; set; }
        public string WebSiteTransactionId { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public decimal WebSiteKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public bool Durum { get; set; } 
        public string? IptalAciklama { get; set; } = string.Empty;
        public DateTime? OnayTarihi { get; set; }
        public bool IsSendToWebSite { get; set; }
        public string WebSiteCallBackResult { get; set; } = string.Empty;

        public WebSiteleri WebSiteleri { get; set; }
    }
}
