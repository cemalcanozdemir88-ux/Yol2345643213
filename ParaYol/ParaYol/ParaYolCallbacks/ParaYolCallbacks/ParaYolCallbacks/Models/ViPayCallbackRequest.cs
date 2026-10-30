using System.Text.Json.Serialization;

namespace ParaYolCallbacks.Models
{
    public class ViPayCallbackRequest
    {
        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
