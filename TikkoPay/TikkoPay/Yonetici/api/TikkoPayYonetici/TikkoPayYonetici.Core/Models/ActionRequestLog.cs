namespace TikkoPayYonetici.Core.Models
{
    public class ActionRequestLog
    {
        public Guid Id { get; set; }
        public Guid? KullaniciId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IpAddress { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
