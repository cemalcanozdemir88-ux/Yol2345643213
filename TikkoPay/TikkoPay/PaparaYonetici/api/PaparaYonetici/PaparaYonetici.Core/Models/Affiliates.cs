namespace PaparaYonetici.Core.Models
{
    public class Affiliates : BaseModel
    {
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }

        public ICollection<WebSiteleri> WebSiteleri { get; set; }
        public ICollection<Kullanicilar> Kullanicilar { get; set; }
    }
}
