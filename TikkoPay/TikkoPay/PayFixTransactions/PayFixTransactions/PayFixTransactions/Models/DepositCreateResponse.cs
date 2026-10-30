namespace PayFixTransactions.Models
{
    public class DepositCreateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PaymentFrameUrl { get; set; }
    }
}
