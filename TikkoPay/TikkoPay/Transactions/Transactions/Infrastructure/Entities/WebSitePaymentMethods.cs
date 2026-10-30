namespace Transactions.Infrastructure.Entities
{
    public class WebSitePaymentMethods
    {
        public Guid Id { get; set; }
        public Guid WebSiteleriId { get; set; }
        public int PaymentMethodId { get; set; }
        public string ApiClientId { get; set; }
        public string ApiClientSecret { get; set; }
        public string DepositCallbackUrl { get; set; }
        public string WithdrawCallbackUrl { get; set; }
        public int Type { get; set; }
        public DateTime KayitTarihi { get; set; }
        public Guid? KaydedenKullaniciId { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
