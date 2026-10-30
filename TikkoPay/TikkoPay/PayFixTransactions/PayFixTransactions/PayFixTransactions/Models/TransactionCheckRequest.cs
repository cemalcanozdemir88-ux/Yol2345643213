namespace PayFixTransactions.Models
{
    public class TransactionCheckRequest
    {
        public string TransactionId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
