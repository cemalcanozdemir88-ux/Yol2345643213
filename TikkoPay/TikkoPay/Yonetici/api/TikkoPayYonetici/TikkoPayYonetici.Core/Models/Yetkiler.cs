namespace TikkoPayYonetici.Core.Models
{
    public class Yetkiler : BaseModel
    {
        public Guid? UstId { get; set; }
        public int SiraNumarasi { get; set; }
        public string Baslik { get; set; } = string.Empty;

        public ICollection<KullaniciYetkileri> KullaniciYetkileri { get; set; }
    }
}
