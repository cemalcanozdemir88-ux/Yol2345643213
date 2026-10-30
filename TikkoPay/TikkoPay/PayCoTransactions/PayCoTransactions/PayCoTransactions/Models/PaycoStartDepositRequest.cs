using System.Text.Json.Serialization;

namespace PayCoTransactions.Models
{
    public class PaycoStartDepositRequest
    {
        public string secret { get; set; }

        public string amount { get; set; }

        public string player_username { get; set; }

        public string tx_id { get; set; }

        public string player_id { get; set; }

        public string player_fullname { get; set; }
    }
}
