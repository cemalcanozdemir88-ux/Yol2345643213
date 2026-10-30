using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class FluxFinansCallbackRequest
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public FluxFinansCallbackRequestData Data { get; set; }
    }

    public class FluxFinansCallbackRequestData
    {
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("finance_id")]
        public int FinanceId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
