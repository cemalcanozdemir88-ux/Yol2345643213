namespace ParaYolYonetici.Core.DTOs.WebSiteleri
{
    public class WebSiteBakiyeListDto
    {
        public Guid Id { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public decimal ToplamYatirim { get; set; }
        public decimal ToplamYatirimKomisyon { get; set; }
        public decimal ToplamCekim { get; set; }
        public decimal ToplamCekimKomisyon { get; set; }
        public decimal ToplamKasaHaraketiGiris { get; set; }
        public decimal ToplamKasaHaraketiCikis { get; set; }
    }
}
