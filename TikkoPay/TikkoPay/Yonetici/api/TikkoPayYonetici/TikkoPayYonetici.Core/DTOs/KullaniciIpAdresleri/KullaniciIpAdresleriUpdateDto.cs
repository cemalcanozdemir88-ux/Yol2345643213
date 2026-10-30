namespace TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri
{
    public class KullaniciIpAdresleriUpdateDto
    {
        public Guid Id { get; set; }
        public Guid KullaniciId { get; set; }
        public string IpAdresi { get; set; } = string.Empty;
    }
}
