using System.Text.Json.Serialization;

namespace TedarikciHizmet.Core.DTOs.Imgbb
{
    public class ImgbbImage
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mime")]
        public string Mime { get; set; }

        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
