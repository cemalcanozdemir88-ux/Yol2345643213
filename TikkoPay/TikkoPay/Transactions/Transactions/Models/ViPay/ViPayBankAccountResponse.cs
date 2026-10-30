namespace Transactions.Models.ViPay
{
    public class ViPayBankAccountResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public ViPayFastDepositData data { get; set; }
    }

    public class ViPayFastDepositData
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
