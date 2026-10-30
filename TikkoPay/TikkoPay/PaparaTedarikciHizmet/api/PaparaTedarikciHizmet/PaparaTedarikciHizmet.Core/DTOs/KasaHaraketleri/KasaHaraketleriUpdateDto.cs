using PaparaTedarikciHizmet.Core.Enums;

namespace PaparaTedarikciHizmet.Core.DTOs.KasaHaraketleri
{
    public class KasaHaraketleriUpdateDto
    {
        public Guid Id { get; set; }
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
