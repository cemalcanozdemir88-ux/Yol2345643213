namespace TikkoPayYonetici.Core.Models
{
    /// <summary>
    /// Transactions projesinin dahili endpoint'i (API'li tedarikçilere yatırım isteği atmak için).
    /// InternalApiKey, Transactions appsettings.json içindeki "InternalApiKey" ile aynı olmalıdır.
    /// </summary>
    public class TransactionsApiSettings
    {
        public string Url { get; set; } = string.Empty;
        public string InternalApiKey { get; set; } = string.Empty;
    }
}
