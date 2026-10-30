namespace PayFixTransactions.Models
{
    public class WithdrawCreateRequest
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public string FullName { get; set; }
        public float Amount { get; set; }
        public string PayfixId { get; set; }
        public string PayfixFullName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Hash { get; set; }
    }
}
