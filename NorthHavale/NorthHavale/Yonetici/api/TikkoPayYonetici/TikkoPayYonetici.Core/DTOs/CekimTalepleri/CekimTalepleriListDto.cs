using TikkoPayYonetici.Core.Enums;

namespace TikkoPayYonetici.Core.DTOs.CekimTalepleri
{
    public class CekimTalepleriListDto : CekimTalepleriBaseListDto
    {
        public Guid? TedarikciId { get; set; }
        public Guid? BankaId { get; set; }
        public string TedarikciAdi { get; set; } = string.Empty;
        public string BankaAdi { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public decimal TedarikciKomisyon { get; set; }
        public decimal Toplam { get; set; }
        public IslemTip IslemTipi { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
        public Durum Durum { get; set; }
        public string GuncelleyenKullanici { get; set; } = string.Empty;
        public string YonlendirenKullanici { get; set; } = string.Empty;
        public DateTime? YonlendirilmeTarihi { get; set; }
        public bool CokluTedarikciyeYonlendirildi { get; set; }
        public Guid? WithdrawTransactionId { get; set; }
    }
}
