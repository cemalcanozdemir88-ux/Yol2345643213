namespace PayCoYonetici.Core.DTOs.Affiliate
{
    public class AffiliateCreateDto
    {
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
    }
}
