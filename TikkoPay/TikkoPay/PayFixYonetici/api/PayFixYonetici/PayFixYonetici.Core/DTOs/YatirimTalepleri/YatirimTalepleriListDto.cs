namespace PayFixYonetici.Core.DTOs.YatirimTalepleri
{
    public class YatirimTalepleriListDto : YatirimTalepleriBaseListDto
    {
        public decimal Toplam { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
    }
}
