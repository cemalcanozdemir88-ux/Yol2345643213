namespace Transactions.Models
{
    public class ErrorLogModel
    {
        public string Exception { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
