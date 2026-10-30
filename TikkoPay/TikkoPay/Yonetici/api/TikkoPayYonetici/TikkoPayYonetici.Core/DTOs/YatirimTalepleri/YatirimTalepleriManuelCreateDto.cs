namespace TikkoPayYonetici.Core.DTOs.YatirimTalepleri
{
    public class YatirimTalepleriManuelCreateDto
    {
        public Guid HavaleBankaId { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
    }
}
