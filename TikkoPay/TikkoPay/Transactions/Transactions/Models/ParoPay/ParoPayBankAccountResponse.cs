using Newtonsoft.Json;

namespace Transactions.Models.ParoPay
{
    public class ParoPayBankAccountResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("ibanOwner")]
        public string IbanOwner { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
