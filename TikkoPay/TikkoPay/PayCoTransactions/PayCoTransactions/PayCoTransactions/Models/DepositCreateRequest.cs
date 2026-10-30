namespace PayCoTransactions.Models
{
    public class DepositCreateRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string Hash { get; set; }
    }
}
