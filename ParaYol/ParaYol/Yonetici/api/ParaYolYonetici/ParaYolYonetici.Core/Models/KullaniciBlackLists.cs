namespace ParaYolYonetici.Core.Models
{
    public class KullaniciBlackLists : BaseModel
    {
        public Guid WebSiteId { get; set; }

        public long UserId { get; set; }

        public WebSiteleri WebSiteleri { get; set; }
    }
}
