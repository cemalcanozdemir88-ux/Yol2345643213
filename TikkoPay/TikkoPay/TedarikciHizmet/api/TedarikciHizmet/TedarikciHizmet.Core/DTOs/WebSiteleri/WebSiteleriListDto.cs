namespace TedarikciHizmet.Core.DTOs.WebSiteleri
{
    public class WebSiteleriListDto
    {
        public Guid Id { get; set; }
        public string WebSiteUrl { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double OdemeKomisyonOrani { get; set; }
    }
}
