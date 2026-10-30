namespace PayCoTransactions.Models
{
    public class PaycoWithDrawRequest
    {
        public string secret { get; set; }
        public string player_username { get; set; }
        public string player_ceppay_wallet_id { get; set; }
        public string player_ceppay_wallet_fullname { get; set; }
        public string amount { get; set; }
        public string tx_id { get; set; }
    }
}
