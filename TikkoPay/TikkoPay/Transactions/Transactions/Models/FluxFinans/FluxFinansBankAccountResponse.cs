using Newtonsoft.Json;

namespace Transactions.Models.FluxFinans
{
    public class FluxFinansBankAccountResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TransactionData Data { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("finance_id")]
        public int FinanceId { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("account")]
        public AccountInfo Account { get; set; }
    }

    public class AccountInfo
    {
        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("account_holder_name")]
        public string AccountHolderName { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("method_type")]
        public string MethodType { get; set; }
    }
}
