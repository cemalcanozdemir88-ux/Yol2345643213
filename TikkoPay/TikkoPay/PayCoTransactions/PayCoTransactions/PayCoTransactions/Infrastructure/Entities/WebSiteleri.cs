namespace PayCoTransactions.Infrastructure.Entities
{
    public partial class WebSiteleri
    {
        public Guid Id { get; set; }

        public int Type { get; set; }

        public string WebSiteUrl { get; set; }

        public double YaririmKomisyonOrani { get; set; }

        public double CekimKomisyonOrani { get; set; }

        public double OdemeKomisyonOrani { get; set; }

        public string ApiClientId { get; set; }

        public string ApiClientSecret { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }

        public string DepositCallbackUrl { get; set; }

        public string WithdrawCallbackUrl { get; set; }

    }
}
