namespace TikkoPayYonetici.Core.Models
{
    public class PaymentMethods
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
        public bool SilindiMi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public Guid? KaydedenKullaniciId { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public Guid? GuncelleyenKullaniciId { get; set; }

        public ICollection<WebSitePaymentMethods> WebSitePaymentMethods { get; set; }
    }
}
