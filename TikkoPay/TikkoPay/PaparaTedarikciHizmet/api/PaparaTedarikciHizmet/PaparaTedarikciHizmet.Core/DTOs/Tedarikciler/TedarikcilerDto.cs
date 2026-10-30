namespace PaparaTedarikciHizmet.Core.DTOs.Tedarikciler
{
    public class TedarikcilerDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public bool BankaHavale { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public bool Durum { get; set; }
    }
}
