namespace NorthHavaleTransactions.Models
{
    public class NhBankAccountResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public NhFastDepositData data { get; set; }
    }

    public class NhFastDepositData
    {
        public string payment_id { get; set; }
        public string account_holder { get; set; }
        public string bank { get; set; }
        public string iban { get; set; }
        public string reference { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string member_id { get; set; }
    }
}
