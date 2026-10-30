namespace PayFixYonetici.Core.DTOs.CekimTalepleri
{
    public class CekimTalepleriListDto : CekimTalepleriBaseListDto
    {
        public decimal Toplam { get; set; }
        public string? IptalAciklama { get; set; } = string.Empty;
    }
}
