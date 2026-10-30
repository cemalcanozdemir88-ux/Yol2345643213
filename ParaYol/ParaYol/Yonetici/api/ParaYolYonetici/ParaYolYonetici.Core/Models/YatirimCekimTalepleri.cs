using ParaYolYonetici.Core.Enums;

namespace ParaYolYonetici.Core.Models
{
    public class YatirimCekimTalepleri : BaseModel
    {
        public Guid? TopId { get; set; }
        public int IdNumber { get; set; }
        public string TalepNo { get; set; } = string.Empty;
        public int? PaymentMethodId { get; set; }
        public Guid? WebSitePaymentMethodId { get; set; }
        public Guid? TedarikciId { get; set; }
        public Guid? HavaleBankaId { get; set; }
        public Guid? BankaId { get; set; }
        public Guid? WebSiteId { get; set; }
        public string WebSiteTransactionId { get; set; } = string.Empty;
        public long? WebSiteTransactionIdBigIntValue { get; set; }
        public Guid? WithdrawTransactionId { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string? AdSoyadYatirimYapan { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public long? UserIdBigIntValue { get; set; }
        public string YontemTipi { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public decimal TedarikciKomisyon { get; set; }
        public decimal WebSiteKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public IslemTip IslemTipi { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
        public int? DekontKontrolDurumu { get; set; }
        public Guid? DekontKontrolKullaniciId { get; set; }
        public DateTime? DekontKontrolTarihi { get; set; }
        public Durum Durum { get; set; }
        public bool CokluTedarikciyeYonlendirildi { get; set; }
        public bool IsSendToWebSite { get; set; }
        public string WebSiteCallBackResult { get; set; } = string.Empty;
        public DateTime? OnayTarihi { get; set; }

        public Guid? YonlendirenKullaniciId { get; set; }
        public DateTime? YonlendirilmeTarihi { get; set; }

        public Tedarikciler Tedarikciler { get; set; }
        public Bankalar Bankalar { get; set; }
        public WebSiteleri WebSiteleri { get; set; }

        public Kullanicilar? DekontKontrolKullanici { get; set; }
        public Kullanicilar GuncelleyenKullanici { get; set; }
        public Kullanicilar YonlendirenKullanici { get; set; }

        public YatirimCekimTalepleri DeepClone()
        {
            return new YatirimCekimTalepleri
            {
                TopId = this.TopId,
                TalepNo = this.TalepNo,
                PaymentMethodId = this.PaymentMethodId,
                WebSitePaymentMethodId = this.WebSitePaymentMethodId,
                HavaleBankaId = this.HavaleBankaId,
                BankaId = this.BankaId,
                WebSiteId = this.WebSiteId,
                WebSiteTransactionId = this.WebSiteTransactionId,
                WebSiteTransactionIdBigIntValue = this.WebSiteTransactionIdBigIntValue,
                WithdrawTransactionId = this.WithdrawTransactionId,
                AdSoyad = this.AdSoyad,
                UserName = this.UserName,
                UserId = this.UserId,
                UserIdBigIntValue = this.UserIdBigIntValue,
                YontemTipi = this.YontemTipi,
                HesapSahibi = this.HesapSahibi,
                HesapNo = this.HesapNo,
                Tutar = this.Tutar,
                TedarikciKomisyon = this.TedarikciKomisyon,
                WebSiteKomisyon = this.WebSiteKomisyon,
                Toplam = this.Toplam,
                IslemTipi = this.IslemTipi,
                IptalAciklama = this.IptalAciklama,
                Durum = this.Durum,
                SilindiMi = this.SilindiMi,
                KayitTarihi = this.KayitTarihi,
                KaydedenKullaniciId = this.KaydedenKullaniciId,
                YonlendirenKullaniciId = this.YonlendirenKullaniciId,
                YonlendirilmeTarihi = this.YonlendirilmeTarihi,
                IsSendToWebSite = this.IsSendToWebSite,
                WebSiteCallBackResult = this.WebSiteCallBackResult,
                CokluTedarikciyeYonlendirildi = this.CokluTedarikciyeYonlendirildi,
            };
        }

    }
}
