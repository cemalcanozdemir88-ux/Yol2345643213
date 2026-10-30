namespace PayFixTransactions.Models
{
    public class PayfixWithDrawRequest
    {
        public string secret { get; set; }
        public string transaction_id { get; set; }
        public string amount { get; set; }
        public string enduser_username { get; set; }
        public string enduser_remote_id { get; set; }
        public string enduser_remote_fullname { get; set; }
        public string payment_short_code { get; set; }
        public string enduser_userId { get; set; }
    }
}
