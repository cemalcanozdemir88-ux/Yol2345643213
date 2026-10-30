using PayCoYonetici.Core.Enums;

namespace PayCoYonetici.Core.DTOs.KasaHaraketleri
{
    public class KasaHaraketleriCreateDto
    {
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizKuru { get; set; }
        public decimal UsdtMiktar { get; set; }
        public string UsdtAdres { get; set; }
        public string UsdtTxtNo { get; set; }
        public string ResimUrl { get; set; }
        public Durum Durum { get; set; }
        public int IslemTipi { get; set; }
    }
}
