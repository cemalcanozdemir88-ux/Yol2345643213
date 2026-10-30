using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.FluxFinans
{
    public class FluxFinansTokenResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TokenData Data { get; set; }
    }

    public class TokenData
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
