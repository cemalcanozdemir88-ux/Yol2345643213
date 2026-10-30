using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.FluxFinans
{
    public class FluxFinansWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TransactionData Data { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }
    }
}
