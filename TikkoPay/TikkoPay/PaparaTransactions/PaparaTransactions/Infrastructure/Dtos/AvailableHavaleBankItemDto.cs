namespace Transactions.Infrastructure.Dtos
{
    public class AvailableHavaleBankItemDto
    {
        public Guid Id { get; set; }
        public string HesapSahibi { get; set; }
        public string HesapNo { get; set; }
        public string Amount { get; set; }
        public double TimeLeft { get; set; }
    }
}
