using Newtonsoft.Json;

namespace TikkoPayYonetici.Core.DTOs.AsPay
{
    public class AsPayTokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }
    }
}
