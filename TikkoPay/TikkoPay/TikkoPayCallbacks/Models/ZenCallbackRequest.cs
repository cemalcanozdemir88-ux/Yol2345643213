using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class ZenDepositCallbackRequest
    {
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("transaction")]
        public Transaction Transaction { get; set; }
    }

    public class Transaction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("usercode")]
        public string Usercode { get; set; }

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

        [JsonPropertyName("account")]
        public Account Account { get; set; } = default!;
    }

    public class Account
    {
        [JsonPropertyName("account_holder")]
        public string AccountHolder { get; set; }

        [JsonPropertyName("account_bank_id")]
        public int? AccountBankId { get; set; }

        [JsonPropertyName("account_bank_name")]
        public string? AccountBankName { get; set; }

        [JsonPropertyName("account_no")]
        public string AccountNo { get; set; }

        [JsonPropertyName("account_iban")]
        public string AccountIban { get; set; }

        [JsonPropertyName("account_branch")]
        public string AccountBranch { get; set; }
    }
}
