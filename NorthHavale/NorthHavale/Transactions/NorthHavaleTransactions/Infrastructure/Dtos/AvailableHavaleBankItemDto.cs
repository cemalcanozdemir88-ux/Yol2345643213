namespace NorthHavaleTransactions.Infrastructure.Dtos
{
    public class AvailableHavaleBankItemDto
    {
        public Guid Id { get; set; }
        public Guid TedarikciId { get; set; }
        public string HesapSahibi { get; set; }
        public string HesapNo { get; set; }
        public string Amount { get; set; }
        public double TimeLeft { get; set; }
        public string WebSiteUrl { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string AutoSupplierBankName { get; set; }
        public bool IsAutoSupplierBankAccount { get; set; }
    }
}
