namespace PayCoYonetici.Core.DTOs.Affiliate
{
    public class AffiliateUpdateDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
    }
}
