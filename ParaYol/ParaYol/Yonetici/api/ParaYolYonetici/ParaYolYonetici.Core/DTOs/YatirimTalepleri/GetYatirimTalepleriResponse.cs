namespace ParaYolYonetici.Core.DTOs.YatirimTalepleri
{
    public class GetYatirimTalepleriResponse
    {
        public int Count { get; set; }
        public List<YatirimTalepleriListDto> YatirimTalepleriListDto { get; set; } = new List<YatirimTalepleriListDto>();
    }
}
