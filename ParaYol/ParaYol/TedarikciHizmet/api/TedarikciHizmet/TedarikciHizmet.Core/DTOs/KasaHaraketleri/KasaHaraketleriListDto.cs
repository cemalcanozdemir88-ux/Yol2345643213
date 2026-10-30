using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Core.DTOs.KasaHaraketleri
{
    public class KasaHaraketleriListDto
    {
        public Guid Id { get; set; }
        public Guid TedarikciId { get; set; }
        public string TedarikciAdi { get; set; } = string.Empty;
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizKuru { get; set; }
        public decimal UsdtMiktar { get; set; }
        public string UsdtAdres { get; set; }
        public string UsdtTxtNo { get; set; }
        public string ResimUrl { get; set; }
        public Durum Durum { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string KaydedenKullanici { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string GuncelleyenKullanici { get; set; }
        public KasaHaraketleriIslemTipi IslemTipi { get; set; }
    }
}
