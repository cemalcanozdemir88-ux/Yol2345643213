using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.ViPay
{
    public class ViPayWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("data")]
        public ViPayWithdraw Data { get; set; }
    }

    public class ViPayWithdraw
    {
        [JsonProperty("payment-id")]
        public string PaymentId { get; set; }

        [JsonProperty("reference-id")]
        public string ReferenceId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("member-id")]
        public string MemberId { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }
    }
}
