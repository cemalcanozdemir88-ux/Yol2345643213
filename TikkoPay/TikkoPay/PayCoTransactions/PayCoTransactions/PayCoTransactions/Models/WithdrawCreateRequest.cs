namespace PayCoTransactions.Models
{
    public class WithdrawCreateRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string TransactionId { get; set; }
        public float Amount { get; set; }
        public string PaycoWalletId { get; set; }
        public string PaycoWalletFullName { get; set; }
        public string Hash { get; set; }
    }
}
