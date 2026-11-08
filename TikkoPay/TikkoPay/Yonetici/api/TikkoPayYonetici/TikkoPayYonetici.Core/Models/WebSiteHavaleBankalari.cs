namespace TikkoPayYonetici.Core.Models
{
    // Web sitesine bağlanan banka havale hesabı.
    // Bir siteye bir tedarikçinin hesabı bağlanırsa o sitede o tedarikçinin sadece bağlanan hesapları kullanılır.
    public class WebSiteHavaleBankalari : BaseModel
    {
        public Guid WebSiteId { get; set; }
        public Guid HavaleBankaId { get; set; }

        public WebSiteleri WebSiteleri { get; set; }
        public HavaleBankalari HavaleBankalari { get; set; }
    }
}
