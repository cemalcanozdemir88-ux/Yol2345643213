namespace NorthHavaleTransactions.Infrastructure.Entities
{
    public class KullaniciBlackLists
    {
        public Guid Id { get; set; }

        public Guid WebSiteId { get; set; }

        public long UserId { get; set; }

        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public Guid? KaydedenKullaniciId { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
