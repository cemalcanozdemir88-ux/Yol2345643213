namespace TikkoPayYonetici.Core.DTOs.CekimTalepleri
{
    public class GetCekimTalepleriResponse
    {
        public int Count { get; set; }
        public List<CekimTalepleriListDto> CekimTalepleriListDto { get; set; } = new List<CekimTalepleriListDto>();
    }
}
