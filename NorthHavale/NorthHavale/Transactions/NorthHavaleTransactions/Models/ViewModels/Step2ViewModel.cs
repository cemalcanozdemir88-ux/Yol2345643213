namespace NorthHavaleTransactions.Models.ViewModels
{
    public class Step2ViewModel
    {
        public Guid Id { get; set; }
        public Guid Transaction { get; set; }
        public Guid BankaId { get; set; }
        public int Amount { get; set; }
        public string HesapSahibi { get; set; }
        public string HesapNo { get; set; }
    }
}
