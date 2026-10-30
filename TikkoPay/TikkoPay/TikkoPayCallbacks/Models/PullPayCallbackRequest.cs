using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class PullPayCallbackRequest
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("update_date")]
        public string? UpdateDate { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
