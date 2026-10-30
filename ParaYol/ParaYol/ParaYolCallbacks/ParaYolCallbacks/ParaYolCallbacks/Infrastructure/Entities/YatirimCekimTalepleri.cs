namespace ParaYolCallbacks.Infrastructure.Entities
{
    public partial class YatirimCekimTalepleri
    {
        public Guid Id { get; set; }

        public Guid? TopId { get; set; }

        public int IdNumber { get; set; }

        public string TalepNo { get; set; }

        public Guid? TedarikciId { get; set; }

        public Guid? WebSitePaymentMethodId { get; set; }

        public Guid? HavaleBankaId { get; set; }

        public Guid? BankaId { get; set; }

        public Guid? WebSiteId { get; set; }

        public string WebSiteTransactionId { get; set; }

        public long? WebSiteTransactionIdBigIntValue { get; set; }

        public Guid? DepositTransactionId { get; set; }

        public string AdSoyad { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public long? UserIdBigIntValue { get; set; }

        public string YontemTipi { get; set; }

        public string HesapSahibi { get; set; }

        public string HesapNo { get; set; }

        public decimal Tutar { get; set; }

        public decimal TedarikciKomisyon { get; set; }

        public decimal WebSiteKomisyon { get; set; }

        public decimal Toplam { get; set; }

        public int IslemTipi { get; set; }

        public int Durum { get; set; }

        public DateTime? OnayTarihi { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }

        public bool IsSendToWebSite { get; set; }

        public string WebSiteCallBackResult { get; set; }

    }
}
