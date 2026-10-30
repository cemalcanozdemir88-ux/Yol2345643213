namespace TikkoPayYonetici.Core.Models
{
    public class Tedarikciler : BaseModel
    {
        public Tedarikciler()
        {
        }

        public string Unvan { get; set; } = string.Empty;
        public bool BankaHavale { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public decimal ToplamYatirim { get; set; }
        public decimal ToplamYatirimKomisyon { get; set; }
        public decimal ToplamCekim { get; set; }
        public decimal ToplamCekimKomisyon { get; set; }
        public decimal ToplamKasaHaraketiGiris { get; set; }
        public decimal ToplamKasaHaraketiCikis { get; set; }
        public decimal AnlikBakiye { get; set; }
        public decimal BakiyeLimitor { get; set; }
        public int YatirimPuani { get; set; }
        public int HesapNoGosterimSayisi { get; set; }
        public bool Durum { get; set; }

        public ICollection<Kullanicilar> Kullanicilar { get; set; }
        public ICollection<HavaleBankalari> HavaleBankalari { get; set; }
        public ICollection<YatirimCekimTalepleri> YatirimTalepleri { get; set; }
        public ICollection<KasaHaraketleri> KasaHaraketleriList { get; set; }
    }
}
