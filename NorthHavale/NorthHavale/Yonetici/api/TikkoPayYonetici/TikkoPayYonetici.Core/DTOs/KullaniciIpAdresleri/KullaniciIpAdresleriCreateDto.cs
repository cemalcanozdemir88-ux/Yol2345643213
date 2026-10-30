namespace TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri
{
    public class KullaniciIpAdresleriCreateDto
    {
        public Guid KullaniciId { get; set; }
        public string IpAdresi { get; set; } = string.Empty;
    }
}
