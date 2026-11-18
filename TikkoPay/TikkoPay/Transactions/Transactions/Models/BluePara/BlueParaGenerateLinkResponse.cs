namespace Transactions.Models.BluePara
{
    public class BlueParaGenerateLinkResponse
    {
        public string transactionId { get; set; }
        public string paymentUrl { get; set; }
        public string status { get; set; }
        public string merchant_transaction_id { get; set; }
        public BlueParaBankAccount bankAccount { get; set; }

        // Hata durumunda dolar
        public bool? success { get; set; }
        public string message { get; set; }
        public List<BlueParaFieldError> errors { get; set; }
    }

    public class BlueParaBankAccount
    {
        public string bankName { get; set; }
        public string iban { get; set; }
        public string accountHolderName { get; set; }
    }

    public class BlueParaFieldError
    {
        public string field { get; set; }
        public string message { get; set; }
    }
}
