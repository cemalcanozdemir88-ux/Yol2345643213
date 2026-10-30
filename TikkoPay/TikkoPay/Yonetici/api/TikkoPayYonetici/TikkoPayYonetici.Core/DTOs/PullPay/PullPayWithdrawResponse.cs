using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.PullPay
{
    public class PullPayWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
