namespace PayFixTransactions.Infrastructure.Entities
{
    public class YatirimTalepleri
    {
        public Guid Id { get; set; }
        public int IdNumber { get; set; }
        public string TalepNo { get; set; }
        public Guid WebSiteId { get; set; }
        public string WebSiteTransactionId { get; set; }
        public string AdSoyad { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public decimal Tutar { get; set; }
        public decimal WebSiteKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public string IptalAciklama { get; set; }
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
