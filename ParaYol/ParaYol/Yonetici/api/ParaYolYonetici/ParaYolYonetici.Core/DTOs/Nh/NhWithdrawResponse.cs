using Newtonsoft.Json;

namespace ParaYolYonetici.Core.DTOs.Nh
{
    public class NhWithdrawResponse
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
