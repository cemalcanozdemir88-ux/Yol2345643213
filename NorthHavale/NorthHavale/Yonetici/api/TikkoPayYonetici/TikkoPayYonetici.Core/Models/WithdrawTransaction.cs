namespace TikkoPayYonetici.Core.Models
{
    public class WithdrawTransaction
    {
        public Guid Id { get; set; }
        public Guid WebSiteId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string Iban { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Hash { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
