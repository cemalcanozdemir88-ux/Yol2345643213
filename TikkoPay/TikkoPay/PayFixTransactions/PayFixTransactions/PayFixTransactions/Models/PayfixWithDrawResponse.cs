namespace PayFixTransactions.Models
{
    public class PayfixWithDrawResponse
    {
        public string status { get; set; }
        public string customer_transaction_id { get; set; }
        public string payment_status { get; set; }
        public string message { get; set; }
        public PayfixWithDrawError[] errors { get; set; }
    }

    public class PayfixWithDrawError
    {
        public string key { get; set; }
        public string message { get; set; }
    }
}
