using Newtonsoft.Json;

namespace NorthHavaleTransactions.Models.PullPay
{
    public class PullPayBankAccountResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("account_iban")]
        public string AccountIban { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
