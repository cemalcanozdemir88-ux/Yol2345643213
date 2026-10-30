namespace NorthKriptoTransactions.Models
{
    public class TransactionCheckResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
