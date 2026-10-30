namespace TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri
{
    public class UpdateIpListDto
    {
        public Guid KullaniciId { get; set; }
        public string[] IpAdresleri { get; set; }
    }
}
