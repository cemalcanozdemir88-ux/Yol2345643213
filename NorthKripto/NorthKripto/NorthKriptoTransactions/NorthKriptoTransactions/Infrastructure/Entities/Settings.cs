namespace NorthKriptoTransactions.Infrastructure.Entities
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MinYatirimTutari { get; set; }
        public string CuzdanYasakliListesi { get; set; } = string.Empty;
    }
}
