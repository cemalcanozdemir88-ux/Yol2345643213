using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class TumBankCallbackRequest
    {
        [JsonPropertyName("sid")]
        public int Sid { get; set; }

        [JsonPropertyName("service")]
        public string Service { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("trx")]
        public string Trx { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
