using System.Text.Json.Serialization;

namespace TedarikciHizmet.Core.DTOs.Imgbb
{
    public class ImgbbUploadResponse
    {
        [JsonPropertyName("data")]
        public ImgbbData Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
