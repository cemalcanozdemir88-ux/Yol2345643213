using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace ImageUpload.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public class UploadRequest
        {
            public required string Base64Image { get; set; }
        }

        [HttpPost("upload")]
        public IActionResult UploadImage([FromBody] UploadRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64Image))
                return BadRequest("Dosya verisi eksik.");

            try
            {
                var match = Regex.Match(request.Base64Image, @"^data:(?<type>.+?);base64,(?<data>.+)$");

                if (!match.Success)
                    return BadRequest("Geçersiz base64 formatı.");

                var mimeType = match.Groups["type"].Value;          
                var base64Data = match.Groups["data"].Value;
                var fileBytes = Convert.FromBase64String(base64Data);

                var extension = GetFileExtension(mimeType);          
                if (string.IsNullOrWhiteSpace(extension))
                    return BadRequest("Desteklenmeyen dosya türü.");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                System.IO.File.WriteAllBytes(filePath, fileBytes);

                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { Url = fileUrl });
            }
            catch
            {
                return StatusCode(500, "Yükleme sırasında hata oluştu.");
            }
        }

        private static string GetFileExtension(string mimeType)
        {
            return mimeType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/jpg" => ".jpg",
                "application/pdf" => ".pdf",
                _ => string.Empty
            };
        }

    }
}
