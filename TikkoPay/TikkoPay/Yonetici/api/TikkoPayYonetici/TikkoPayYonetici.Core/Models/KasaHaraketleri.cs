using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Core.Models
{
    public class KasaHaraketleri : BaseModel
    {
        public Guid? TopId { get; set; }
        public Guid TedarikciId { get; set; }
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizKuru { get; set; }
        public decimal UsdtMiktar { get; set; }
        public string UsdtAdres { get; set; }
        public string UsdtTxtNo { get; set; }
        public string ResimUrl { get; set; }
        public KasaHaraketleriIslemTipi IslemTipi { get; set; }
        public Durum Durum { get; set; }
        public Tedarikciler Tedarikciler { get; set; }

        public Kullanicilar KaydedenKullanici { get; set; }
        public Kullanicilar GuncelleyenKullanici { get; set; }
    }
}
