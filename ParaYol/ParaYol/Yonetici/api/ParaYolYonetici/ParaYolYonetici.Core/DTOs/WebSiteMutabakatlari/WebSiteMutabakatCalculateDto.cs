namespace ParaYolYonetici.Core.DTOs.WebSiteMutabakatlari
{
    public class WebSiteMutabakatCalculateDto
    {
        public decimal Devir { get; set; }
        public Guid WebSiteId { get; set; }
        public string MutabakatTarihi { get; set; } = string.Empty;
    }
}
