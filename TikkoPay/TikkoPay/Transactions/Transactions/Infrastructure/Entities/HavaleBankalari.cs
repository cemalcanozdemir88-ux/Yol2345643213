namespace Transactions.Infrastructure.Entities
{
    public partial class HavaleBankalari
    {
        public Guid Id { get; set; }

        public Guid TedarikciId { get; set; }

        public Guid BankaId { get; set; }

        public string HesapSahibi { get; set; }

        public string HesapNo { get; set; }

        public decimal HesapMinTutar { get; set; }

        public decimal HesapMaxTutar { get; set; }

        public int MaxYatirimAdedi { get; set; }

        public decimal MaxYatirimToplami { get; set; }

        public int OnaylananYatirimAdedi { get; set; }

        public decimal OnaylananYatirimToplami { get; set; }

        public bool Durum { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
