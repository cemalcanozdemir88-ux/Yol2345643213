namespace TikkoPayYonetici.Core.Models
{
    public class KullaniciIpAdresleri : BaseModel
    {
        public Guid KullaniciId { get; set; }
        public string IpAdresi { get; set; } = string.Empty;

        public Kullanicilar Kullanicilar { get; set; }
    }
}
