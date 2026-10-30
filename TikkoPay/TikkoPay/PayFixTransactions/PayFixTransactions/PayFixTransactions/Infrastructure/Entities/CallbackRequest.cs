namespace PayFixTransactions.Infrastructure.Entities
{
    public class CallbackRequest
    {
        public Guid Id { get; set; }
        public string PaymentShortCode { get; set; }
        public string TransactionId { get; set; }
        public string EndUserUsername { get; set; }
        public string FullName { get; set; }
        public string Amount { get; set; }
        public string Token { get; set; }
        public string CustomerTransactionId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
