namespace TikkoPayYonetici.Core.Models
{
    public class WebSiteTedarikcileri : BaseModel
    {
        public Guid WebSiteId { get; set; }
        public Guid TedarikciId { get; set; }

        public WebSiteleri WebSiteleri { get; set; }
        public Tedarikciler Tedarikciler { get; set; }
    }
}
