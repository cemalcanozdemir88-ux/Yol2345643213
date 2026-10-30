namespace TikkoPayYonetici.Core.DTOs.CekimTalepleri
{
    public class CekimTalepleriBaseListDto
    {
        public Guid? Id { get; set; }
        public string TalepNoWithFormated { get; set; } = string.Empty;
        public int? PaymentMethodId { get; set; }
        public string WebSiteTransactionId { get; set; } = string.Empty;
        public string WebSiteUrl { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public DateTime? OnayTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
    }
}
