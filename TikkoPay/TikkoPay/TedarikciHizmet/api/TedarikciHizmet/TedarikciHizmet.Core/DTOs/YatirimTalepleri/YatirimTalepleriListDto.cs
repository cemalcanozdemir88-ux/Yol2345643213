using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Core.DTOs.YatirimTalepleri
{
    public class YatirimTalepleriListDto
    {
        public Guid? Id { get; set; }
        public string TalepNoWithFormated { get; set; }
        public Guid? TedarikciId { get; set; }
        public Guid? BankaId { get; set; }
        public string TedarikciAdi { get; set; } = string.Empty;
        public string BankaAdi { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public string HesapNoBanka { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public decimal TedarikciKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public IslemTip IslemTipi { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
        public Durum Durum { get; set; }
        public DateTime? OnayTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string GuncelleyenKullanici { get; set; } = string.Empty;
    }
}
