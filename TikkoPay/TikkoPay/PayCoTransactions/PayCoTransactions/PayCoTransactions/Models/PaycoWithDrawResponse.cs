namespace PayCoTransactions.Models
{
    public class PaycoWithDrawResponse
    {
        public string status { get; set; }
        public long our_tx_id { get; set; }
        public string your_tx_id { get; set; }
        public string message { get; set; }
    }
}
