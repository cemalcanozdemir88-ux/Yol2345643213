using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.TumBank
{
    public class TumBankWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
