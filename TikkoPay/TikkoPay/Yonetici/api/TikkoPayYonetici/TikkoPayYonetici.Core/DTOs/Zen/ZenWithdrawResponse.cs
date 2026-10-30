using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.Zen
{
    public class ZenWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        public bool Success { get; set; }

        [JsonProperty("transaction")]
        public TransactionDetail Transaction { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class TransactionDetail
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("usercode")]
        public string Usercode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; } 

        [JsonProperty("confirmedAmount")]
        public string? ConfirmedAmount { get; set; } 

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("statusText")]
        public string StatusText { get; set; }

        [JsonProperty("statusDateTime")]
        public DateTime StatusDateTime { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }
}
