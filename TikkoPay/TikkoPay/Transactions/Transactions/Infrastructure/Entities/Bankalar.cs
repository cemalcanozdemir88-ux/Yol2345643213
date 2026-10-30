namespace Transactions.Infrastructure.Entities
{
    public partial class Bankalar
    {
        public Guid Id { get; set; }

        public string Baslik { get; set; }

        public string Resim { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
