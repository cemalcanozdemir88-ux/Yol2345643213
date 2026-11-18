using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.BluePara
{
    public class BlueParaWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("errors")]
        public List<BlueParaFieldError>? Errors { get; set; }

        [JsonProperty("data")]
        public BlueParaWithdrawData? Data { get; set; }
    }

    public class BlueParaWithdrawData
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("merchant_transaction_id")]
        public string? MerchantTransactionId { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class BlueParaFieldError
    {
        [JsonProperty("field")]
        public string? Field { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
