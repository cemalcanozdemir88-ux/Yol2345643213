namespace Transactions.Models.ViewModels
{
    public class FastOptionsViewModel
    {
        public Guid TransactionId { get; set; }
        public string WebsiteTitle { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
