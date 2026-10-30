using System.Text;
using System.Text.Json;
using TedarikciHizmet.Core.DTOs.Imgbb;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class ImgbbUploadService : IImgbbUploadService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ImgbbUploadService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadImageAsync(byte[] fileContent)
        {
            string apiKey = "96421e4fbb39680d691d354d2b2471a1";
            var base64Image = Convert.ToBase64String(fileContent);

            var client = _httpClientFactory.CreateClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(apiKey), "key" },
                { new StringContent(base64Image), "image" }
            };

            var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
            var result = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var imgbbResponse = JsonSerializer.Deserialize<ImgbbUploadResponse>(result, options);
            return imgbbResponse?.Data?.Image?.Url;
        }

        public async Task<List<string>> UploadImagesAsync(IEnumerable<byte[]> files)
        {
            var resultList = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var url = await UploadImageAsync(file);
                    if (!string.IsNullOrEmpty(url))
                    {
                        resultList.Add(url);
                    }

                    await Task.Delay(500); 
                }
                catch
                {
                    // Loglama yapılabilir
                    resultList.Add(null);
                }
            }

            return resultList;
        }
    }
}
