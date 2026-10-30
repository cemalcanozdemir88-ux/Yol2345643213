namespace PayCoYonetici.Core.DTOs.Affiliate
{
    public class AffiliateBakiyeListDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public decimal ToplamYatirim { get; set; }
        public decimal ToplamYatirimKomisyon { get; set; }
        public decimal ToplamCekim { get; set; }
        public decimal ToplamCekimKomisyon { get; set; }
    }
}
