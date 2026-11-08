namespace TikkoPayYonetici.Core.DTOs.WebSiteHavaleBankalari
{
    public class WebSiteHavaleBankalariDto
    {
        public Guid Id { get; set; }
        public Guid WebSiteId { get; set; }
        public Guid HavaleBankaId { get; set; }
        public Guid TedarikciId { get; set; }
        public string TedarikciBaslik { get; set; } = string.Empty;
        public string BankaBaslik { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public decimal HesapMinTutar { get; set; }
        public decimal HesapMaxTutar { get; set; }
        public bool Durum { get; set; }
    }

    public class WebSiteHavaleBankalariCreateDto
    {
        public Guid WebSiteId { get; set; }
        public Guid HavaleBankaId { get; set; }
    }
}
