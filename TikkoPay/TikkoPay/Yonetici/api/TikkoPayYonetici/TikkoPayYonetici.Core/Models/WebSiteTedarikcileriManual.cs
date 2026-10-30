namespace TikkoPayYonetici.Core.Models
{
    public class WebSiteTedarikcileriManual : BaseModel
    {
        public Guid WebSiteId { get; set; }
        public Guid TedarikciId { get; set; }

        public WebSiteleri WebSiteleri { get; set; }
        public Tedarikciler Tedarikciler { get; set; }
    }
}
