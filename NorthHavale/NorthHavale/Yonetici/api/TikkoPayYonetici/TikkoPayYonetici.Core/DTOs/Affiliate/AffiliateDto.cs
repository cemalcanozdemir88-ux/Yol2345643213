namespace TikkoPayYonetici.Core.DTOs.Affiliate
{
    public class AffiliateDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }

    }
}
