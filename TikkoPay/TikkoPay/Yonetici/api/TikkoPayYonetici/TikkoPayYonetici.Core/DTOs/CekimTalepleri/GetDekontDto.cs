namespace TikkoPayYonetici.Core.DTOs.CekimTalepleri
{
    public class GetDekontDto
    {
        public Guid? Id { get; set; }
        public int DekontKontrolDurumu { get; set; }
        public string DekontKontrolKullanici { get; set; } = string.Empty;
        public string DekontKontrolTarihi { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
    }
}
