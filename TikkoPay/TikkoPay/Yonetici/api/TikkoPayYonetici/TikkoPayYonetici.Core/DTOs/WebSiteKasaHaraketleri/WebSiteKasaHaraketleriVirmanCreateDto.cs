namespace TikkoPayYonetici.Core.DTOs.WebSiteKasaHaraketleri
{
    public class WebSiteKasaHaraketleriVirmanCreateDto
    {
        public Guid FromWebSitesiId { get; set; }
        public int FromVirmanIslemTipi { get; set; }
        public Guid ToWebSitesiId { get; set; }
        public int ToVirmanIslemTipi { get; set; }

        public decimal Tutar { get; set; }
        public decimal DovizKuru { get; set; }
        public decimal UsdtMiktar { get; set; }
        public string UsdtAdres { get; set; }
        public string UsdtTxtNo { get; set; }
        public string ResimUrl { get; set; }
        public string Aciklama { get; set; }
    }
}
