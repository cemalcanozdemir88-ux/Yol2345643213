namespace TikkoPayYonetici.Core.DTOs.WebSiteTedarikcileriManual
{
    public class WebSiteTedarikcileriManualDto
    {
        public Guid Id { get; set; }
        public Guid WebSiteId { get; set; }
        public Guid TedarikciId { get; set; }
        public string WebSiteBaslik { get; set; } = string.Empty;
        public string TedarikciBaslik { get; set; } = string.Empty;
    }
}
