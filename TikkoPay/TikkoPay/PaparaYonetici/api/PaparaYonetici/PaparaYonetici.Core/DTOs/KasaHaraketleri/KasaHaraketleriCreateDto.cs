using PaparaYonetici.Core.Enums;

namespace PaparaYonetici.Core.DTOs.KasaHaraketleri
{
    public class KasaHaraketleriCreateDto
    {
        public Guid TedarikciId { get; set; }
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
