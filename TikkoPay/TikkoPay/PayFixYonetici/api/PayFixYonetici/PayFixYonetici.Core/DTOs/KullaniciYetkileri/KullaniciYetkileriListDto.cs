namespace PayFixYonetici.Core.DTOs.KullaniciYetkileri
{
    public class KullaniciYetkileriListDto
    {
        public Guid YetkiId { get; set; }
        public Guid KullaniciId { get; set; }
        public Guid? UstId { get; set; }
        public int SiraNumarasi { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public bool KullaniciYetkiliMi { get; set; }

    }
}
