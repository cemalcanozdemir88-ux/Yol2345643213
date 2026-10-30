using ParaYolYonetici.Core.Enums;

namespace ParaYolYonetici.Core.DTOs.YatirimTalepleri
{
    public class YatirimTalepleriCreateDto
    {
        public Guid? TedarikciId { get; set; }
        public Guid? HavaleBankaId { get; set; }
        public Guid WebSiteId { get; set; }
        public int PaymentMethodId { get; set; }
        public string HesapNo { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IptalAciklama { get; set; } = string.Empty;
        public Durum Durum { get; set; }
        public IslemTip IslemTipi { get; set; }
        public decimal Tutar { get; set; }
    }
}
