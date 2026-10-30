using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Core.DTOs.YatirimTalepleri
{
    public class YatirimTalepleriUpdateDto
    {
        public Guid Id { get; set; }
        public Guid? TedarikciId { get; set; }
        public Guid BankaId { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public double Komisyon { get; set; }
        public decimal Toplam { get; set; }
        public IslemTip IslemTipi { get; set; }
        public string IptalAciklama { get; set; } = string.Empty;
        public Durum Durum { get; set; }
        public DateTime? OnayTarihi { get; set; }
    }
}
