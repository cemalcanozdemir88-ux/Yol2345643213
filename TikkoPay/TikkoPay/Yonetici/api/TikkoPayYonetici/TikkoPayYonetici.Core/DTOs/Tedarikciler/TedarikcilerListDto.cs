namespace TikkoPayYonetici.Core.DTOs.Tedarikciler
{
    public class TedarikcilerListDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public bool BankaHavale { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public double TedarikcidenAlinanOdemeKomisyonOrani { get; set; }
        public decimal AnlikBakiye { get; set; }
        public int YatirimPuani { get; set; }
        public int HesapNoGosterimSayisi { get; set; }
        public decimal MinYatirimTutari { get; set; }
        public decimal BakiyeLimitor { get; set; }
        public int FastHesapSayisi { get; set; }
        public bool OtoPuan { get; set; }
        public bool Durum { get; set; }
    }
}
