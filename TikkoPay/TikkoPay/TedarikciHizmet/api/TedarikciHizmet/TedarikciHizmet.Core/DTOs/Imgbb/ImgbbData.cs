using System.Text.Json.Serialization;

namespace TedarikciHizmet.Core.DTOs.Imgbb
{
    public class ImgbbData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url_viewer")]
        public string UrlViewer { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("display_url")]
        public string DisplayUrl { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("expiration")]
        public int Expiration { get; set; }

        [JsonPropertyName("image")]
        public ImgbbImage Image { get; set; }

        [JsonPropertyName("thumb")]
        public ImgbbImage Thumb { get; set; }

        [JsonPropertyName("medium")]
        public ImgbbImage Medium { get; set; }

        [JsonPropertyName("delete_url")]
        public string DeleteUrl { get; set; }
    }

}
