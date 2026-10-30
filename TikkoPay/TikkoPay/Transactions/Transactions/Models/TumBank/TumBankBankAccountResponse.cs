using Newtonsoft.Json;

namespace Transactions.Models.TumBank
{
    public class TumBankBankAccountResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public ApiData Data { get; set; }
    }

    public class ApiData
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("account_holder")]
        public string AccountHolder { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }
}
