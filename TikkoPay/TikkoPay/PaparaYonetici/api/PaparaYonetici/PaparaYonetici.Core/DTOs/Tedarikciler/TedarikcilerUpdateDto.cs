namespace PaparaYonetici.Core.DTOs.Tedarikciler
{
    public class TedarikcilerUpdateDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public bool BankaHavale { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public int YatirimPuani { get; set; }
        public bool Durum { get; set; }
    }
}
