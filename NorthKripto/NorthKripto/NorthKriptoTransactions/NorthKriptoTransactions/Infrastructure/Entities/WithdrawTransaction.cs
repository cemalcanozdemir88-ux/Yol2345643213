namespace NorthKriptoTransactions.Infrastructure.Entities
{
    public class WithdrawTransaction
    {
        public Guid Id { get; set; }

        public Guid WebSiteId { get; set; }

        public Guid WebSitePaymentMethodId { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string TransactionId { get; set; }

        public string Amount { get; set; }

        public string WalletAddress { get; set; }

        public string WalletAddressType { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Hash { get; set; }

        public DateTime KayitTarihi { get; set; }
    }
}
