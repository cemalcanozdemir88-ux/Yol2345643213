namespace PaparaTedarikciHizmet.Core.DTOs.Tedarikciler
{
    public class TedarikcilerCreateDto
    {
        public string Unvan { get; set; } = string.Empty;
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public bool Durum { get; set; }
    }
}
