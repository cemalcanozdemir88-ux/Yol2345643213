namespace NorthHavaleTransactions.Infrastructure.Entities
{
    public partial class Tedarikciler
    {
        public Guid Id { get; set; }

        public string Unvan { get; set; }

        public bool BankaHavale { get; set; }

        public int KalanLimit { get; set; }

        public decimal IslemLimit { get; set; }

        public decimal GercekBakiye { get; set; }

        public decimal BakiyeLimit { get; set; }

        public double YaririmKomisyonOrani { get; set; }

        public double CekimKomisyonOrani { get; set; }

        public bool Durum { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
