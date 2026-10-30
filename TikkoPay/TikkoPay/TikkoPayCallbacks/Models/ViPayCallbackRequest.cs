using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    //public class ViPayCallbackRequest
    //{
    //    [JsonPropertyName("status")]
    //    public bool Status { get; set; }

    //    [JsonPropertyName("message")]
    //    public string Message { get; set; }

    //    [JsonPropertyName("data")]
    //    public ViPayCallbackRequestData Data { get; set; }
    //}

    //public class ViPayCallbackRequestData
    //{
    //    [JsonPropertyName("transaction_id")]
    //    public string TransactionId { get; set; }

    //    [JsonPropertyName("user_id")]
    //    public string UserId { get; set; }

    //    [JsonPropertyName("amount")]
    //    public string Amount { get; set; }

    //    [JsonPropertyName("hash")]
    //    public string Hash { get; set; }
    //}

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
