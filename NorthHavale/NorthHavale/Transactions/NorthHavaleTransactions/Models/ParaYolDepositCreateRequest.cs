using Newtonsoft.Json;

namespace NorthHavaleTransactions.Models
{
    public class ParaYolDepositCreateRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("transactionid")]
        public string TransactionId { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("clientid")]
        public string ClientId { get; set; }

        [JsonProperty("clientsecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}
