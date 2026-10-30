namespace ParaYolYonetici.Core.DTOs.YatirimTalepleri
{
    public class OnaySuresiDto
    {
        public int Saat { get; set; }
        public int Dakika { get; set; }
        public int Saniye { get; set; }
        public string FormatliSure => $"{Saat:D2} sa {Dakika:D2} dk {Saniye:D2} sn";
    }
}
