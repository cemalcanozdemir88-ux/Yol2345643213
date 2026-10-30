using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class ZenWithdrawCallbackRequest
    {
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("transaction")]
        public TransactionDetail Transaction { get; set; }
    }

    public class TransactionDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("usercode")]
        public string Usercode { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("confirmedAmount")]
        public string ConfirmedAmount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("statusText")]
        public string StatusText { get; set; }

        [JsonPropertyName("statusDateTime")]
        public string StatusDateTime { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
    }
}
