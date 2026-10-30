using PayCoYonetici.Core.Enums;

namespace PayCoYonetici.Core.Models
{
    public class KasaHaraketleri : BaseModel
    {
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizKuru { get; set; }
        public decimal UsdtMiktar { get; set; }
        public string UsdtAdres { get; set; }
        public string UsdtTxtNo { get; set; }
        public string ResimUrl { get; set; }
        public KasaHaraketleriIslemTipi IslemTipi { get; set; }
        public Durum Durum { get; set; }

        public Kullanicilar KaydedenKullanici { get; set; }
        public Kullanicilar GuncelleyenKullanici { get; set; }
    }
}

