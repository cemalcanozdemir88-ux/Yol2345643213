using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class ParoPayCallbackRequest
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
