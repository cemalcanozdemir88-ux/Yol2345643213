using Newtonsoft.Json;

namespace NorthHavaleTransactions.Models
{
    public class ParaYolWithdrawCreateRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("transactionid")]
        public string TransactionId { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("clientid")]
        public string ClientId { get; set; }

        [JsonProperty("clientsecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}
