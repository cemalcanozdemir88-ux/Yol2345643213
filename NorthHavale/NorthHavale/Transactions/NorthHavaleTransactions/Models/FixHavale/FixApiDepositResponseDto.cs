using System.Text.Json.Serialization;

namespace NorthHavaleTransactions.Models.FixHavale
{
    public interface IFixApiResponse
    {
        bool IsSuccessStatusCode { get; set; }
    }

    public sealed class FixApiDepositResponseDto : IFixApiResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("detectedIp")]
        public string DetectedIp { get; set; }

        [JsonIgnore]
        public bool IsSuccessStatusCode { get; set; }

        [JsonPropertyName("transaction")]
        public FixApiDepositTransactionDto Transaction { get; set; }

        [JsonPropertyName("bankAccount")]
        public FixApiBankAccountDto BankAccount { get; set; }
    }

    public sealed class FixApiDepositTransactionDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("transactionNumber")] public int TransactionNumber { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("userId")] public string UserId { get; set; }
        [JsonPropertyName("username")] public string Username { get; set; }
        [JsonPropertyName("firstName")] public string FirstName { get; set; }
        [JsonPropertyName("middleName")] public string MiddleName { get; set; }
        [JsonPropertyName("lastName")] public string LastName { get; set; }
        [JsonPropertyName("partnerSite")] public string PartnerSite { get; set; }
        [JsonPropertyName("siteId")] public string SiteId { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("bankAccountId")] public string BankAccountId { get; set; }
        [JsonPropertyName("panelId")] public string PanelId { get; set; }
        [JsonPropertyName("notes")] public string Notes { get; set; }
        [JsonPropertyName("externalTransactionId")] public string ExternalTransactionId { get; set; }
        [JsonPropertyName("externalSiteId")] public string ExternalSiteId { get; set; }
        [JsonPropertyName("externalSiteName")] public string ExternalSiteName { get; set; }
    }

    public sealed class FixApiBankAccountDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("bankName")] public string BankName { get; set; }
        [JsonPropertyName("accountName")] public string AccountName { get; set; }
        [JsonPropertyName("iban")] public string Iban { get; set; }
        [JsonPropertyName("expiresAt")] public DateTimeOffset ExpiresAt { get; set; }
    }

    public sealed class FixApiErrorDto
    {
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("detectedIp")] public string DetectedIp { get; set; }
    }
}
