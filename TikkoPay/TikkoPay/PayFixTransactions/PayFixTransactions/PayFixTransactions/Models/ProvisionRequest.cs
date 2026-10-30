using System.Text.Json.Serialization;

namespace PayFixTransactions.Models
{
    public class ProvisionRequest
    {
        [JsonPropertyName("secret")]
        public string secret { get; set; }

        [JsonPropertyName("amount")]
        public string amount { get; set; }

        [JsonPropertyName("enduser_userId")]
        public string enduser_userId { get; set; }

        [JsonPropertyName("enduser_username")]
        public string enduser_username { get; set; }

        [JsonPropertyName("enduser_fullname")]
        public string enduser_fullname { get; set; }

        [JsonPropertyName("customer_transaction_id")]
        public string customer_transaction_id { get; set; }

        [JsonPropertyName("payment_short_code")]
        public string payment_short_code { get; set; }
    }
}
