namespace TikkoPayCallbacks.Infrastructure.Dtos
{
    public class TedarikciHavaleBankalariDto
    {
        public Guid Id { get; set; }
        public string Tedarikci { get; set; } = null!;
        public decimal TedarikciBakiyeLimitor { get; set; }
        public string BankaAdi { get; set; } = null!;
        public decimal HesapMinTutar { get; set; }
        public decimal HesapMaxTutar { get; set; }
        public string HesapSahibi { get; set; } = null!;
        public string HesapNo { get; set; } = null!;
    }
}
