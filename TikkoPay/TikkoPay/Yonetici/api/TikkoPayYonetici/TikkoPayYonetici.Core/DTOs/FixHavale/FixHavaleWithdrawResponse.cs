using System.Transactions;
using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.FixHavale
{
    public class FixHavaleWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        public bool Ok { get; set; }
        public FixHavaleWithdrawResponseTransaction Transaction { get; set; }
    }

    public class FixHavaleWithdrawResponseTransaction
    {
        public string Id { get; set; }
        public int TransactionNumber { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PartnerSite { get; set; }
        public string SiteId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PanelId { get; set; }
        public string Notes { get; set; }
        public FixHavaleWithdrawResponseUserBankAccount UserBankAccount { get; set; }
        public string ExternalTransactionId { get; set; }
        public string ExternalSiteId { get; set; }
        public string ExternalSiteName { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class FixHavaleWithdrawResponseUserBankAccount
    {
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string Iban { get; set; }
    }
}
