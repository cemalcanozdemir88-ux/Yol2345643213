namespace PayCoTransactions.Infrastructure.Entities
{
    public class CallbackRequest
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string TransactionId { get; set; }
        public string PlayerUsername { get; set; }
        public string Amount { get; set; }
        public string PlayerFullname { get; set; }
        public string Token { get; set; }
        public string CustomerTransactionId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
