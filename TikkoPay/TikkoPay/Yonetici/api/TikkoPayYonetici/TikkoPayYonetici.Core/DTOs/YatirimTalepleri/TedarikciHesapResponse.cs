namespace TikkoPayYonetici.Core.DTOs.YatirimTalepleri
{
    /// <summary>
    /// Transactions "api/internal/deposit/send-to-supplier" cevabı:
    /// API'li tedarikçinin bu yatırım için verdiği hesap bilgisi.
    /// </summary>
    public class TedarikciHesapResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? HavaleBankaId { get; set; }
        public Guid? BankaId { get; set; }
        public string HesapNo { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
    }
}
