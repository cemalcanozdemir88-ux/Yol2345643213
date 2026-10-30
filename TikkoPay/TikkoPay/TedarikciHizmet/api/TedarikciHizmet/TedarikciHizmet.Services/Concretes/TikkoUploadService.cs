using System.Text;
using System.Text.Json;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class TikkoUploadService : ITikkoUploadService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TikkoUploadService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadImageAsync(byte[] fileContent, string contentType)
        {
            var base64 = Convert.ToBase64String(fileContent);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "96421e4fbb39680d691d354d2b2471a1");

            var request = new
            {
                Base64Image = $"data:{contentType};base64,{base64}"
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://159.100.17.233/api/image/upload", content);
            var result = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var uploadResponse = JsonSerializer.Deserialize<UploadResponse>(result, options);

            return uploadResponse?.Url;
        }

        public async Task<List<string>> UploadImagesAsync(IEnumerable<(byte[] File, string ContentType)> files)
        {
            var resultList = new List<string>();

            foreach (var (file, contentType) in files)
            {
                try
                {
                    var url = await UploadImageAsync(file, contentType);
                    if (!string.IsNullOrEmpty(url))
                    {
                        resultList.Add(url);
                    }

                    await Task.Delay(100);
                }
                catch
                {
                    resultList.Add(null); 
                }
            }

            return resultList;
        }

        public class UploadResponse
        {
            public string Url { get; set; }
        }

    }
}
