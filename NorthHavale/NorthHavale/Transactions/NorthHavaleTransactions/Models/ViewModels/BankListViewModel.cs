using NorthHavaleTransactions.Infrastructure.Entities;

namespace NorthHavaleTransactions.Models.ViewModels
{
    public class BankListViewModel
    {
        public Guid TransactionId { get; set; }
        public string WebsiteTitle { get; set; }
        public IEnumerable<Bankalar> Bankalar { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
