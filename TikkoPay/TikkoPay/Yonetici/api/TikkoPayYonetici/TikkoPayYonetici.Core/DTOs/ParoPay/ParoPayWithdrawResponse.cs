using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.ParoPay
{
    public class ParoPayWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
