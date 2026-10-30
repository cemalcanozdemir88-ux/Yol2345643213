namespace PayCoTransactions.Models
{
    public class PaycoCallbackRequest
    {
        public string type { get; set; }
        public long cp_tx_id { get; set; }
        public string player_username { get; set; }
        public float amount { get; set; }
        public string player_fullname { get; set; }
        public string token { get; set; }
        public string your_tx_id { get; set; }
        public string status { get; set; }
        public string description { get; set; }
    }
}
