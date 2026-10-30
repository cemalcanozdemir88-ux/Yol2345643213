namespace Transactions.Models.ViewModels
{
    public class TransactionDetailViewModel
    {
        public string NameSurname { get; set; }
        public string Amount { get; set; }
        public List<BankViewModel> Banks { get; set; }
    }

    public class BankViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
