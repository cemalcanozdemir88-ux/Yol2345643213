namespace ParaYolYonetici.Core.DTOs.KullaniciIpAdresleri
{
    public class KullaniciIpAdresleriWithAvailableCount
    {
        public int IpAdresSayisi { get; set; }
        public List<KullaniciIpAdresleriListDto> kullaniciIpAdresleriListDtos { get; set; } = new();
    }
}
