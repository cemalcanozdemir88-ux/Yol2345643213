using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Core.Models
{
    public class YatirimCekimTalepleriManual : BaseModel
    {
        public Guid? TopId { get; set; }
        public int IdNumber { get; set; }
        public string TalepNo { get; set; } = string.Empty;
        public Guid? WebSitePaymentMethodId { get; set; }
        public Guid? TedarikciId { get; set; }
        public Guid? HavaleBankaId { get; set; }
        public Guid? BankaId { get; set; }
        public Guid? WebSiteId { get; set; }
        public string WebSiteTransactionId { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public string HesapNoBanka { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public decimal TedarikciKomisyon { get; set; }
        public decimal WebSiteKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public IslemTip IslemTipi { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
        public Durum Durum { get; set; }
        public bool IsSendToWebSite { get; set; }
        public string WebSiteCallBackResult { get; set; } = string.Empty;
        public DateTime? OnayTarihi { get; set; }

        public Tedarikciler Tedarikciler { get; set; }
        public Bankalar Bankalar { get; set; }
        public WebSiteleri WebSiteleri { get; set; }

        public Kullanicilar GuncelleyenKullanici { get; set; }
    }
}
