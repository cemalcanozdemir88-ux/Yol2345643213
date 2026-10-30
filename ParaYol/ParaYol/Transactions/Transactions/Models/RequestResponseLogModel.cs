namespace Transactions.Models
{
    public class RequestResponseLogModel
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public string ResponseTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
