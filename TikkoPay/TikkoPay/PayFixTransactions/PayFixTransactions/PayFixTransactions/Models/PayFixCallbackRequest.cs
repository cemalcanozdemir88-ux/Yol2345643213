using System.Text.Json.Serialization;

namespace PayFixTransactions.Models
{
    public class PayFixCallbackRequest
    {
        [JsonPropertyName("payment_short_code")]
        public string payment_short_code { get; set; }

        [JsonPropertyName("transaction_id")]
        public string transaction_id { get; set; }

        [JsonPropertyName("enduser_username")]
        public string enduser_username { get; set; }

        [JsonPropertyName("fullName")]
        public string fullName { get; set; }

        [JsonPropertyName("amount")]
        public string amount { get; set; }

        [JsonPropertyName("token")]
        public string token { get; set; }

        [JsonPropertyName("customer_transaction_id")]
        public string customer_transaction_id { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }
    }
}
